using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Extension;
using Adin.BankPayment.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Adin.BankPayment.Controllers
{
    public class MellatController : Controller
    {
        private readonly ILogger<MellatController> _logger;
        private readonly IRepository<Transaction> _transactionRepository;

        public MellatController(ILogger<MellatController> logger,
            IRepository<Transaction> transactionRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
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

            var transaction = await _transactionRepository.Get(Guid.Parse(secondTrackCode));

            if (transaction.Status == (byte) TransactionStatusEnum.Success ||
                transaction.Status == (byte) TransactionStatusEnum.Cancel)
                return BadRequest();

            var longUrl = transaction.CallbackUrl;
            var uriBuilder = new UriBuilder(longUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            _logger.LogError(longUrl);
            ErrorCodeEnum bankErrorCode;
            if (string.IsNullOrEmpty(Request.Form["SaleReferenceId"].ToString()))
            {
                bankErrorCode = ErrorCodeEnum.UnkownError;
                var errorMsg = "رسید قابل قبول نیست";
                //ResCode=StatusPayment
                if (!string.IsNullOrEmpty(Request.Form["ResCode"].ToString()))
                {
                    errorMsg = MellatHelper.MellatResult(Request.Form["ResCode"].ToString());
                    bankErrorCode = MellatHelper.ErrorResult(Request.Form["ResCode"].ToString());
                }

                return await ReturnErrorPage(transaction, (byte) bankErrorCode, errorMsg);
            }

            bankErrorCode = ErrorCodeEnum.NoError;
            try
            {
                try
                {
                    var SaleOrderId = long.Parse(Request.Form["SaleOrderId"].ToString().TrimEnd());

                    var SaleReferenceId = long.Parse(Request.Form["SaleReferenceId"].ToString().TrimEnd());

                    var RefId = Request.Form["RefId"].ToString().TrimEnd();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    const string message =
                        " وضعیت:مشکلی در پرداخت بوجود آمده ، در صورتی که وجه پرداختی از حساب بانکی شما کسر شده است آن مبلغ به صورت خودکار برگشت داده خواهد شد ";
                    return await ReturnErrorPage(transaction, (byte) ErrorCodeEnum.UnkownError, message);
                }

                transaction.BankErrorCode = (byte) bankErrorCode;
                transaction.Status = (byte) TransactionStatusEnum.BankOk;
                transaction.ModifiedOn = DateTime.Now;
                transaction.ModifiedBy = 1;

                transaction.BankTrackCode = Request.Form["SaleReferenceId"].ToString();

                await _transactionRepository.Update(transaction);

                query["id"] = transaction.Id.ToString();
                query["trackCode"] = transaction.UserTrackCode;
                query["status"] = true.ToString();
                query["errorCode"] = bankErrorCode.ToString();
                query["message"] = "پرداخت با موفقیت انجام شد";

                uriBuilder.Query = query.ToString();

                longUrl = uriBuilder.ToString();

                var url3 = string.Format(longUrl);
                return Redirect(url3);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                var message =
                    "مشکلی در پرداخت به وجود آمده است ، در صورتیکه وجه پرداختی از حساب بانکی شما کسر شده است آن مبلغ به صورت خودکار برگشت داده خواهد شد";
                return await ReturnErrorPage(transaction, (byte) ErrorCodeEnum.UnkownError, message);
            }
        }

        private void BypassCertificateError()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate { return true; };
        }

        public async Task<IActionResult> ReturnErrorPage(Transaction transaction, byte bankErrorCode,
            string errorMessage = "")
        {
            var longUrl = transaction.CallbackUrl;
            var uriBuilder = new UriBuilder(longUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            transaction.Status = (byte) TransactionStatusEnum.Failed;
            transaction.BankErrorCode = bankErrorCode;
            transaction.BankErrorMessage = errorMessage;
            transaction.ModifiedOn = DateTime.Now;
            transaction.ModifiedBy = 1;
            await _transactionRepository.Update(transaction);

            query["id"] = transaction.Id.ToString();
            query["trackCode"] = transaction.UserTrackCode;
            query["status"] = false.ToString();
            query["errorCode"] = bankErrorCode.ToString();
            query["message"] = errorMessage;
            uriBuilder.Query = query.ToString();
            longUrl = uriBuilder.ToString();
            var url = string.Format(longUrl);
            return Redirect(url);
        }
    }
}