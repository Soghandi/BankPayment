using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Adin.BankPayment.Service;
using Adin.BankPayment.Domain.Model;
using System.Linq;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Extension;
using Adin.BankPayment.Mellat;
using Microsoft.Extensions.Caching.Memory;

namespace Adin.BankPayment.Controllers
{
    // [Route("api/[controller]")]
    public class PayController : Controller
    {
        private readonly ILogger<PayController> _logger;
        private IRepository<Transaction> _transactionRepository;
        private IRepository<Application> _applicationRepository;
        private IRepository<Bank> _bankRepository;
        private IRepository<ApplicationBank> _applicationBankRepository;



        public PayController(ILogger<PayController> logger,
                             IRepository<Transaction> transactionRepository,
                             IRepository<Application> applicationRepository,
                             IRepository<Bank> bankRepository,
                             IRepository<ApplicationBank> applicationBankRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _applicationRepository = applicationRepository;
            _bankRepository = bankRepository;
            _applicationBankRepository = applicationBankRepository;
        }

        [HttpGet("/Pay/{id}")]
        public async Task<IActionResult> Index(Guid id)
        {
            var transaction = await _transactionRepository.Get(id);
            if (transaction == null)
            {
                return Unauthorized();
            }
            if (transaction.ExpirationTime != null && transaction.ExpirationTime < DateTime.Now)
                return Unauthorized();
            var applicationBank = await _applicationBankRepository.GetFirstBy(x => x.ApplicationId == transaction.ApplicationId && x.BankId == transaction.BankId);
            switch (transaction.Bank.Code)
            {

                case (byte)BankCodeEnum.Parsian:
                    _logger.LogInformation("Parsian");
                    var pinParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "ParsianPIN");
                    Parsian.ParsianGateway parsianGateway = new Parsian.ParsianGateway(pinParam.ParamValue);
                    var resp = await parsianGateway.PinPaymentRequest(Convert.ToInt32(transaction.Amount), Convert.ToInt32(transaction.UserTrackCode), transaction.CallbackUrl);
                    var paramsInfo = applicationBank.ApplicationBankParams.ToList();
                    if (resp.Body.status == 0)
                    {
                        _logger.LogInformation("authority after callback", resp.Body.authority);
                        var url = string.Format(applicationBank.Bank.PostUrl, resp.Body.authority);
                        transaction.BankTrackCode = resp.Body.authority.ToString();
                        await _transactionRepository.Update(transaction);
                        return Redirect(url);
                    }
                    else
                    {
                        _logger.LogInformation("Critical Error: Payment Error. StatusCode={0}", resp.Body.status);
                        transaction.BankTrackCode = resp.Body.authority.ToString();
                        switch (resp.Body.status)
                        {
                            case 20:
                            case 22:
                                transaction.ErrorCode = (byte)ErrorCodeEnum.InvalidPin;
                                transaction.BankErrorMessage = "پين فروشنده درست نميباشد";
                                break;
                            case 30:
                                transaction.ErrorCode = (byte)ErrorCodeEnum.OperationAlreadyDone;
                                transaction.BankErrorMessage = "عمليات قبلا با موفقيت انجام شده است";
                                break;
                            case 34:
                                transaction.ErrorCode = (byte)ErrorCodeEnum.UserTrackCodeIsInvalid;
                                transaction.BankErrorMessage = "شماره تراكنش فروشنده درست نميباشد";
                                break;
                        }

                        await _transactionRepository.Update(transaction);
                        return BadRequest(transaction.BankErrorMessage);
                    }
                case (byte)BankCodeEnum.Mellat:
                    _logger.LogInformation("mellat");
                    var termialParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatTerminalId");
                    var userNameParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatUserName");
                    var passwordParam = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatPassword");

                    var terminalID = termialParam.ParamValue;
                    var userName = userNameParam.ParamValue;
                    var password = passwordParam.ParamValue;
                    MellatGateway mellatGateway = new MellatGateway(terminalID, userName, password);
                    var mellatResp = await mellatGateway.bpPayRequest(Convert.ToInt32(transaction.Amount), Convert.ToInt32(transaction.UserTrackCode), transaction.BankRedirectUrl);
                    var mellatParamsInfo = applicationBank.ApplicationBankParams.ToList();
                    _logger.LogError((mellatResp == null).ToString());

                    if (mellatResp != null)
                    {
                        _logger.LogError(mellatResp.Body.@return);

                        string[] ResultArray = mellatResp.Body.@return.Split(',');
                        if (ResultArray[0].ToString() == "0")
                        {

                            var refId = ResultArray[1];
                            transaction.BankTrackCode = refId;
                            await _transactionRepository.Update(transaction);
                            ViewBag.RefId = refId;
                            return View("Mellat", transaction);
                        }
                        else
                        {
                            _logger.LogError("Critical Error: Payment Error. StatusCode={0}", ResultArray[0].ToString());
                            transaction.ErrorCode = (byte)MellatHelper.ErrorResult(ResultArray[0].ToString());
                            transaction.BankErrorCode = Convert.ToInt32(ResultArray[0].ToString());
                            transaction.BankErrorMessage = MellatHelper.MellatResult(ResultArray[0].ToString());
                            await _transactionRepository.Update(transaction);
                            return BadRequest(transaction.BankErrorMessage);
                        }
                    }
                    else
                    {
                        _logger.LogError("Critical Error: Payment Error. StatusCode=no resp");

                        transaction.BankErrorMessage = "امکان اتصال به درگاه بانک وجود ندارد";
                        await _transactionRepository.Update(transaction);
                        return BadRequest(transaction.BankErrorMessage);
                    }
                case (byte)BankCodeEnum.Saman:
                default:
                    _logger.LogInformation("Saman");
                    ViewBag.Params = applicationBank.ApplicationBankParams.ToList();
                    return View("Saman", transaction);
            }
        }


    }
}

