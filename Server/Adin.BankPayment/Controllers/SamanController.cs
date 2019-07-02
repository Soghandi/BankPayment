using System;
using System.Threading.Tasks;
using System.Web;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Adin.BankPayment.Controllers
{
    public class SamanController : Controller
    {
        private readonly ILogger<SamanController> _logger;
        private readonly IRepository<Transaction> _transactionRepository;
        private string _errorMsg = "";
        private string _referenceNumber = string.Empty;
        private string _reservationNumber = string.Empty;
        private string _traceNumber = string.Empty;
        private string _transactionState = string.Empty;

        public SamanController(ILogger<SamanController> logger,
            IRepository<Transaction> transactionRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Callback(string token, string secondTrackCode)
        {
            _logger.LogDebug("CallBack");
            _logger.LogDebug("token:" + token);
            _logger.LogDebug("secondTrackCode:" + secondTrackCode);
            var transaction = await _transactionRepository.Get(Guid.Parse(secondTrackCode));

            if (transaction.Status == (byte)TransactionStatusEnum.Success ||
                transaction.Status == (byte)TransactionStatusEnum.Cancel)
                return BadRequest();

            var longUrl = transaction.CallbackUrl;
            var uriBuilder = new UriBuilder(longUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            var refNum = Request.Form["RefNum"];

            var bankErrorCode = ErrorCodeEnum.UnkownError;
            try
            {
                if (RequestUnpack())
                {
                    _logger.LogDebug($"TransactionId: {secondTrackCode}; _transactionState : {_transactionState}");
                    if (_transactionState.Equals("OK"))
                    {
                        bankErrorCode = ErrorCodeEnum.NoError;
                        transaction.BankErrorCode = (byte)bankErrorCode;
                        transaction.Status = (byte)TransactionStatusEnum.BankOk;
                        transaction.ModifiedOn = DateTime.Now;
                        transaction.ModifiedBy = 1;
                        transaction.BankTrackCode = refNum;
                        await _transactionRepository.Update(transaction);

                        query["id"] = transaction.Id.ToString();
                        query["trackCode"] = transaction.UserTrackCode;
                        query["status"] = true.ToString();
                        query["errorCode"] = ((byte)bankErrorCode).ToString();
                        query["message"] = string.Empty;
                        uriBuilder.Query = query.ToString();
                        longUrl = uriBuilder.ToString();
                        var url3 = string.Format(longUrl);
                        return Redirect(url3);
                    }

                    _errorMsg = "متاسفانه بانک خريد شما را تاييد نکرده است";
                    if (_transactionState.Equals("CanceledByUser") || _transactionState.Equals(string.Empty))
                    {
                        // Transaction was canceled by user
                        _errorMsg = "تراكنش توسط خريدار كنسل شد";
                        bankErrorCode = ErrorCodeEnum.CanceledByUser;
                    }
                    //InvalidParameters
                    else if (_transactionState.Equals("InvalidParameters"))
                    {
                        // Amount of revers teransaction is more than teransaction
                        _errorMsg = "پارامترهای ارسال شده به بانک اشتباه است";
                        bankErrorCode = ErrorCodeEnum.InvalidParameters;
                    }
                    else if (_transactionState.Equals("InvalidAmount"))
                    {
                        // Amount of revers teransaction is more than teransaction
                        _errorMsg = "مبلغ تراکنش معکوس بیشتر از مبلغ تراکنش است";
                        bankErrorCode = ErrorCodeEnum.InvalidAmount;
                    }
                    else if (_transactionState.Equals("InvalidTransaction"))
                    {
                        // Can not find teransaction
                        _errorMsg = "تراکنش معتبر نمی باشد";
                        bankErrorCode = ErrorCodeEnum.InvalidTransaction;
                    }
                    else if (_transactionState.Equals("InvalidCardNumber"))
                    {
                        // Card number is wrong
                        _errorMsg = "شماره کارت معتبر نمی باشد";
                        bankErrorCode = ErrorCodeEnum.InvalidCardNumber;
                    }
                    else if (_transactionState.Equals("NoSuchIssuer"))
                    {
                        // Issuer can not find
                        _errorMsg = "صادر کننده پیدا نشد";
                        bankErrorCode = ErrorCodeEnum.NoSuchIssuer;
                    }
                    else if (_transactionState.Equals("ExpiredCardPickUp"))
                    {
                        // The card is expired
                        _errorMsg = "کارت انتخاب شده منقضی شده است";
                        bankErrorCode = ErrorCodeEnum.ExpiredCardPickUp;
                    }
                    else if (_transactionState.Equals("AllowablePINTriesExceededPickUp"))
                    {
                        // For third time user enter a wrong PIN so card become invalid
                        _errorMsg = "پین انتخاب شده محدودیت کارت دارد";
                        bankErrorCode = ErrorCodeEnum.AllowablePINTriesExceededPickUp;
                    }
                    else if (_transactionState.Equals("IncorrectPIN"))
                    {
                        // Pin card is wrong
                        _errorMsg = "پین کد اشتباه است";
                        bankErrorCode = ErrorCodeEnum.IncorrectPIN;
                    }
                    else if (_transactionState.Equals("ExceedsWithdrawalAmountLimit"))
                    {
                        // Exceeds withdrawal from amount limit
                        _errorMsg = "پرداخت بیشتر از از حد مجاز می باشد";
                        bankErrorCode = ErrorCodeEnum.ExceedsWithdrawalAmountLimit;
                    }
                    else if (_transactionState.Equals("TransactionCannotBeCompleted"))
                    {
                        // PIN and PAD are currect but Transaction Cannot Be Completed
                        _errorMsg = "تراکنش کامل نشد";
                        bankErrorCode = ErrorCodeEnum.TransactionCannotBeCompleted;
                    }
                    else if (_transactionState.Equals("ResponseReceivedTooLate"))
                    {
                        // Timeout occur
                        _errorMsg = "جواب کاربر بیشتر از حد مجاز طول کشید";
                        bankErrorCode = ErrorCodeEnum.ResponseReceivedTooLate;
                    }
                    else if (_transactionState.Equals("SuspectedFraudPickUp"))
                    {
                        // User did not insert cvv2 & expiredate or they are wrong.
                        _errorMsg = "کاربر اطلاعات کارت خود را به درستی وارد نکرده است";
                        bankErrorCode = ErrorCodeEnum.SuspectedFraudPickUp;
                    }
                    else if (_transactionState.Equals("NoSufficientFunds"))
                    {
                        // there are not suficient funds in the account
                        _errorMsg = "موجودی کافی نمی باشد";
                        bankErrorCode = ErrorCodeEnum.NoSufficientFunds;
                    }
                    else if (_transactionState.Equals("IssuerDownSlm"))
                    {
                        // The bank server is down
                        _errorMsg = "سرور بانک غیرفعال است";
                        bankErrorCode = ErrorCodeEnum.BankServerIsDown;
                    }
                    else if (_transactionState.Equals("TMEError"))
                    {
                        // unknown error occur
                        _errorMsg = "خطای ناشناخته";
                        bankErrorCode = ErrorCodeEnum.UnkownError;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;
                _logger.LogError(ex.Message);
            }

            _logger.LogError(_errorMsg);

            transaction.Status = (byte)TransactionStatusEnum.Failed;
            transaction.BankErrorCode = (byte)bankErrorCode;
            transaction.BankErrorMessage = _errorMsg;
            transaction.ModifiedOn = DateTime.Now;
            transaction.ModifiedBy = 1;
            await _transactionRepository.Update(transaction);

            query["id"] = transaction.Id.ToString();
            query["trackCode"] = transaction.UserTrackCode;
            query["status"] = false.ToString();
            query["errorCode"] = ((byte)bankErrorCode).ToString();
            query["message"] = _errorMsg;
            uriBuilder.Query = query.ToString();
            longUrl = uriBuilder.ToString();
            var url = string.Format(longUrl);
            return Redirect(url);
        }

        private bool RequestUnpack()
        {
            //    if (RequestFeildIsEmpty()) return false;
            _logger.LogDebug("requestpack");
            _referenceNumber = Request.Form["RefNum"].ToString();
            _logger.LogDebug(_referenceNumber);
            _reservationNumber = Request.Form["ResNum"].ToString();
            _logger.LogDebug(_reservationNumber);
            _transactionState = Request.Form["state"].ToString();
            _logger.LogDebug(_transactionState);
            _traceNumber = Request.Form["TraceNo"].ToString();
            _logger.LogDebug(_traceNumber);

            return true;
        }
    }
}