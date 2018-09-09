using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Microsoft.Extensions.Logging;
using Adin.BankPayment.Service;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Connector.Enum;
using System.Web;
using Adin.BankPayment.Extension;
using Adin.BankPayment.Mellat;
using System.Collections.Specialized;

namespace Adin.BankPayment.Controllers
{
    public class MellatController : Controller
    {
        private string _referenceNumber = string.Empty;
        private string _reservationNumber = string.Empty;
        private string _transactionState = string.Empty;
        private string _traceNumber = string.Empty;
        private bool _isError = false;
        private string _succeedMsg = "";
        private string _webBaseUrl = "";
        private readonly ILogger<MellatController> _logger;
        private IRepository<Transaction> _transactionRepository;
        private IRepository<Application> _applicationRepository;
        private IRepository<Bank> _bankRepository;
        private IRepository<ApplicationBank> _applicationBankRepository;


        public MellatController(ILogger<MellatController> logger,
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


        [HttpPost]
        public async Task<IActionResult> Callback()
        {

            ViewBag.Bank = "ملت";
            BypassCertificateError();
            var token = Request.Query["token"];
            var secondTrackCode = Request.Query["secondTrackCode"];

            _logger.LogInformation("CallBack");
            _logger.LogInformation("token:" + token);
            _logger.LogInformation("secondTrackCode:" + secondTrackCode);

            _logger.LogInformation(Request.Form["SaleReferenceId"].ToString());
            _logger.LogInformation(Request.Form["ResCode"].ToString());
            _logger.LogInformation(Request.Form["SaleOrderId"].ToString());
            _logger.LogInformation(Request.Form["SaleReferenceId"].ToString());
          

            Transaction transaction = await _transactionRepository.Get(Guid.Parse(secondTrackCode));
            if (transaction.Status != (byte)TransactionStatusEnum.Initial)
            {
                return BadRequest();
            }

            string longurl = transaction.CallbackUrl;
            var uriBuilder = new UriBuilder(longurl);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            _logger.LogError(longurl);
            if (string.IsNullOrEmpty(Request.Form["SaleReferenceId"].ToString()))
            {
                var bankErrorCode = ErrorCodeEnum.UnkownError;
                var _errorMsg = "رسید قابل قبول نیست";
                //ResCode=StatusPayment
                if (!string.IsNullOrEmpty(Request.Form["ResCode"].ToString()))
                {
                    _errorMsg = MellatHelper.MellatResult(Request.Form["ResCode"].ToString());
                    bankErrorCode = MellatHelper.ErrorResult(Request.Form["ResCode"].ToString());

                }

                return await ReturnErrorPage(transaction, (byte)bankErrorCode, _errorMsg);

            }
            else
            {                
                try
                {
                    var applicationBank = await _applicationBankRepository.GetFirstBy(x => x.ApplicationId == transaction.ApplicationId && x.BankId == transaction.BankId);

                    string TerminalId = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatTerminalId").ParamValue;

                    string UserName = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatUserName").ParamValue;
                    string Password = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatPassword").ParamValue;

                    long SaleOrderId = 0;  //PaymentID 
                    long SaleReferenceId = 0;
                    string RefId = null;
                    try
                    {
                        SaleOrderId = long.Parse(Request.Form["SaleOrderId"].ToString().TrimEnd());

                        SaleReferenceId = long.Parse(Request.Form["SaleReferenceId"].ToString().TrimEnd());

                        RefId = Request.Form["RefId"].ToString().TrimEnd();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        var message = " وضعیت:مشکلی در پرداخت بوجود آمده ، در صورتی که وجه پرداختی از حساب بانکی شما کسر شده است آن مبلغ به صورت خودکار برگشت داده خواهد شد ";
                        return await ReturnErrorPage(transaction, (byte)ErrorCodeEnum.UnkownError, message);
                    }


                    var bankErrorCode = ErrorCodeEnum.NoError;
                    transaction.BankErrorCode = (byte)bankErrorCode;
                    transaction.Status = (byte)TransactionStatusEnum.BankOk;
                    transaction.ModifiedOn = DateTime.Now;
                    transaction.ModifiedBy = 1;

                    transaction.BankTrackCode = Request.Form["SaleReferenceId"].ToString();

                    await _transactionRepository.Update(transaction);


                    query["id"] = transaction.Id.ToString();
                    query["trackCode"] = transaction.UserTrackCode.ToString();
                    query["status"] = true.ToString();
                    query["errorCode"] = bankErrorCode.ToString();
                    query["message"] = "پرداخت با موفقیت انجام شد";

                    uriBuilder.Query = query.ToString();

                    longurl = uriBuilder.ToString();

                    var url3 = string.Format(longurl);
                    return Redirect(url3);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    var message = "مشکلی در پرداخت به وجود آمده است ، در صورتیکه وجه پرداختی از حساب بانکی شما کسر شده است آن مبلغ به صورت خودکار برگشت داده خواهد شد";
                    return await ReturnErrorPage(transaction, (byte)ErrorCodeEnum.UnkownError, message);
                }
            }
        }

        void BypassCertificateError()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate (
                    Object sender1,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
        }


        public async Task<IActionResult> ReturnErrorPage(Transaction transaction, byte bankErrorCode, string errorMessage = "")
        {

            string longurl = transaction.CallbackUrl;
            var uriBuilder = new UriBuilder(longurl);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);


            transaction.Status = (byte)TransactionStatusEnum.Failed;
            transaction.BankErrorCode = (byte)bankErrorCode;
            transaction.BankErrorMessage = errorMessage;
            transaction.ModifiedOn = DateTime.Now;
            transaction.ModifiedBy = 1;
            await _transactionRepository.Update(transaction);


            query["id"] = transaction.Id.ToString();
            query["trackCode"] = transaction.UserTrackCode.ToString();
            query["status"] = false.ToString();
            query["errorCode"] = ((byte)bankErrorCode).ToString();
            query["message"] = errorMessage;
            uriBuilder.Query = query.ToString();
            longurl = uriBuilder.ToString();
            var url = string.Format(longurl);
            return Redirect(url);
        }

    }
}
