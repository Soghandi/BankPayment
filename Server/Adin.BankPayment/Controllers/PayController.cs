using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Efarda;
using Adin.BankPayment.Extension;
using Adin.BankPayment.Mellat;
using Adin.BankPayment.Parsian;
using Adin.BankPayment.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Adin.BankPayment.Controllers
{
    public class PayController : Controller
    {
        private readonly IRepository<ApplicationBank> _applicationBankRepository;
        private readonly ILogger<PayController> _logger;
        private readonly IRepository<Transaction> _transactionRepository;

        public PayController(ILogger<PayController> logger,
            IRepository<Transaction> transactionRepository,
            IRepository<ApplicationBank> applicationBankRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _applicationBankRepository = applicationBankRepository;
        }

        [HttpGet("/Pay/{id}")]
        public async Task<IActionResult> Index(Guid id)
        {
            _logger.LogWarning("Pay - Id : " + id);
            var transaction = await _transactionRepository.Get(id);
            if (transaction == null ||
                transaction.Status == (byte)TransactionStatusEnum.Cancel)
                return View("Error", "لینک وارد شده صحیح نیست.");

            if (transaction.Status == (byte)TransactionStatusEnum.Success ||
                transaction.Status == (byte)TransactionStatusEnum.BankOk)
                return View("Error", "این تراکنش قبلا پرداخت شده است.");

            if (transaction.ExpirationTime.HasValue &&
                transaction.ExpirationTime < DateTime.Now)
            {
                var persianCalendar = new PersianCalendar();
                var date =
                    $"{persianCalendar.GetYear(transaction.ExpirationTime.Value)}/{persianCalendar.GetMonth(transaction.ExpirationTime.Value)}/" +
                    $"{persianCalendar.GetDayOfMonth(transaction.ExpirationTime.Value)}" +
                    $" - {transaction.ExpirationTime.Value.Hour}:{transaction.ExpirationTime.Value.Minute}";
                return View("Error",
                    $"این لینک تا تاریخ {date} معتبر بوده است، لطفا از پشتیبانی تقاضای لینک جدید بفرمایید.");
            }

            var applicationBank = await _applicationBankRepository.GetFirstBy(x =>
                x.ApplicationId == transaction.ApplicationId && x.BankId == transaction.BankId);
            switch (transaction.Bank.Code)
            {
                case (byte)BankCodeEnum.Parsian:
                    _logger.LogInformation("Parsian");
                    var pinParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "ParsianPIN");

                    var parsianGateway = new ParsianGateway(pinParam.ParamValue);
                    var resp = await parsianGateway.SalePaymentRequestasync(Convert.ToInt64(transaction.Amount),
                        Convert.ToInt64(transaction.UserTrackCode), transaction.BankRedirectUrl, transaction.Id.ToString());
                    if (resp.Body.SalePaymentRequestResult.Status == 0)
                    {
                        _logger.LogInformation("authority after callback", resp.Body.SalePaymentRequestResult.Token);
                        var url = string.Format(applicationBank.Bank.PostUrl, resp.Body.SalePaymentRequestResult.Token);
                        transaction.BankTrackCode = resp.Body.SalePaymentRequestResult.Token.ToString();
                        await _transactionRepository.Update(transaction);
                        return Redirect(url);
                    }
                    else
                    {
                        _logger.LogInformation("Critical Error: Payment Error. StatusCode={0} - Message={1}", resp.Body.SalePaymentRequestResult.Status, ParsianErrors.GetResult(resp.Body.SalePaymentRequestResult.Status).Message);
                        transaction.BankTrackCode = resp.Body.SalePaymentRequestResult.Token.ToString();
                        switch (resp.Body.SalePaymentRequestResult.Status)
                        {
                            case -126:
                                transaction.ErrorCode = (byte)ErrorCodeEnum.InvalidPin;
                                transaction.BankErrorMessage = "پين فروشنده درست نميباشد";
                                break;
                            case 2:
                            case -1533:
                            case -1536:
                                transaction.ErrorCode = (byte)ErrorCodeEnum.OperationAlreadyDone;
                                transaction.BankErrorMessage = "عمليات قبلا با موفقيت انجام شده است";
                                break;
                            case -112:
                                transaction.ErrorCode = (byte)ErrorCodeEnum.UserTrackCodeIsInvalid;
                                transaction.BankErrorMessage = "شماره تراكنش فروشنده درست نميباشد";
                                break;
                        }

                        await _transactionRepository.Update(transaction);
                        return BadRequest(transaction.BankErrorMessage);
                    }

                case (byte)BankCodeEnum.Mellat:
                    _logger.LogInformation("mellat");
                    var terminalParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatTerminalId");
                    var userNameParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatUserName");
                    var passwordParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatPassword");

                    var mellatGateway = new MellatGateway(terminalParam.ParamValue, userNameParam.ParamValue, passwordParam.ParamValue);
                    var mellatResp = await mellatGateway.BpPayRequest(Convert.ToInt32(transaction.Amount),
                        Convert.ToInt32(transaction.UserTrackCode), transaction.BankRedirectUrl);
                    _logger.LogError((mellatResp == null).ToString());

                    if (mellatResp != null)
                    {
                        _logger.LogError(mellatResp.Body.@return);

                        var resultArray = mellatResp.Body.@return.Split(',');
                        if (resultArray[0] == "0")
                        {
                            var refId = resultArray[1];
                            transaction.BankTrackCode = refId;
                            await _transactionRepository.Update(transaction);
                            ViewBag.RefId = refId;
                            return View("Mellat", transaction);
                        }

                        _logger.LogError("Critical Error: Payment Error. StatusCode={0}", resultArray[0]);
                        transaction.ErrorCode = (byte)MellatHelper.ErrorResult(resultArray[0]);
                        transaction.BankErrorCode = Convert.ToInt32(resultArray[0]);
                        transaction.BankErrorMessage = MellatHelper.MellatResult(resultArray[0]);
                        await _transactionRepository.Update(transaction);
                        return BadRequest(transaction.BankErrorMessage);
                    }
                    else
                    {
                        _logger.LogError("Critical Error: Payment Error. StatusCode=no resp");

                        transaction.BankErrorMessage = "امکان اتصال به درگاه بانک وجود ندارد";
                        await _transactionRepository.Update(transaction);
                        return BadRequest(transaction.BankErrorMessage);
                    }

                case (byte)BankCodeEnum.Efarda:
                    _logger.LogInformation("Efarda");

                    var EfardaServiceId =
                       applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "serviceId");
                    var EfardaUsername =
                        applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "userName");
                    var EfardaPassword =
                        applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "password");

                    EfardaGateway efardaGateway = new EfardaGateway(_logger, EfardaUsername.ParamValue, EfardaPassword.ParamValue, EfardaServiceId.ParamValue);
                    var traceNumber = await efardaGateway.GetTraceId(transaction.UserTrackCode, transaction.Amount.ToString("###############0"), transaction.BankRedirectUrl, transaction.Mobile.HasValue ? transaction.Mobile.Value.ToString() : string.Empty);

                    transaction.BankTrackCode = traceNumber;
                    await _transactionRepository.Update(transaction);

                    ViewBag.Username = EfardaUsername.ParamValue;
                    ViewBag.traceNumber = traceNumber;

                    return View("Efarda", transaction);

                case (byte)BankCodeEnum.Saman:
                default:
                    _logger.LogInformation("Saman");
                    ViewBag.Params = applicationBank.ApplicationBankParams.ToList();
                    return View("Saman", transaction);
            }
        }
    }
}