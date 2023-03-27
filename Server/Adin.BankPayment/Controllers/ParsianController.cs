using System;
using System.Threading.Tasks;
using System.Web;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Parsian;
using Adin.BankPayment.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Adin.BankPayment.Controllers
{
    public class ParsianController : Controller
    {
        private readonly ILogger<ParsianController> _logger;
        private readonly IRepository<Transaction> _transactionRepository;
        private string _errorMsg = "";

        public ParsianController(ILogger<ParsianController> logger,
            IRepository<Transaction> transactionRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Callback(long token, string secondTrackCode)
        {
            //long FormToken = Convert.ToInt64(Request.Form["Token"]);
            var Status = Request.Form["status"];
            //var OrderId = Request.Form["OrderId"];
            //var TerminalNo = Request.Form["TerminalNo"];
            //var RRN = Request.Form["RRN"];
            //var HashCardNumber = Request.Form["HashCardNumber"];
            //var Amount = Request.Form["Amount"];

            var transaction = await _transactionRepository.Get(Guid.Parse(secondTrackCode));

            if (transaction.Status == (byte)TransactionStatusEnum.Success ||
                transaction.Status == (byte)TransactionStatusEnum.Cancel)
                return BadRequest();

            var longUrl = transaction.CallbackUrl;
            var uriBuilder = new UriBuilder(longUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            var bankErrorCode = ErrorCodeEnum.UnkownError;
            try
            {
                var (Success, Message) = ParsianErrors.GetResult(int.Parse(Status));

                if (Success)
                {
                    bankErrorCode = ErrorCodeEnum.NoError;
                    transaction.BankErrorCode = (byte)bankErrorCode;
                    transaction.Status = (byte)TransactionStatusEnum.BankOk;
                    transaction.ModifiedOn = DateTime.Now;
                    transaction.ModifiedBy = 1;
                    await _transactionRepository.Update(transaction);

                    query["id"] = transaction.Id.ToString();
                    query["trackCode"] = transaction.UserTrackCode;
                    query["status"] = true.ToString();
                    query["errorCode"] = ((byte)bankErrorCode).ToString();
                    query["message"] = string.Empty;
                    uriBuilder.Query = query.ToString();
                    return Redirect(string.Format(uriBuilder.ToString()));
                }
                else
                {
                    _errorMsg = $"{Status} - {Message}";
                }
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;
                _logger.LogError(_errorMsg);
            }

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
            return Redirect(string.Format(uriBuilder.ToString()));
        }
    }
}