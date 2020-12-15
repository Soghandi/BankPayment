using System;
using System.Threading.Tasks;
using System.Web;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Efarda;
using Adin.BankPayment.Pasargad;
using Adin.BankPayment.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Adin.BankPayment.Controllers
{
    public class PasargadController : Controller
    {
        private readonly ILogger<PasargadController> _logger;
        private readonly IRepository<Transaction> _transactionRepository;
        private string _errorMsg = "";

        public PasargadController(ILogger<PasargadController> logger,
            IRepository<Transaction> transactionRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Callback(string InvoiceNumber, string InvoiceDate, string TransactionReferenceID)
        {
            var transaction = await _transactionRepository.GetFirstBy(a => a.UserTrackCode == InvoiceNumber && a.CreationDate.ToString() == InvoiceDate);

            if (transaction.Status == (byte)TransactionStatusEnum.Success ||
                transaction.Status == (byte)TransactionStatusEnum.Cancel)
                return BadRequest();


            var merchantCode =
                      applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "merchantCode");
            var terminalCode =
                applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "terminalCode");
            var privateKey =
                applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "privateKey");

            var pasargadGateway = new PasargadGateway(_logger, merchantCode.ParamValue, terminalCode.ParamValue, privateKey.ParamValue);

            var longUrl = transaction.CallbackUrl;
            var uriBuilder = new UriBuilder(longUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            var result = Request.Form["result"];
            var channelRefNumber = Request.Form["channelRefNumber"];
            var traceNumber = Request.Form["traceNumber"];
            var additionalData = Request.Form["additionalData"];

            var bankErrorCode = ErrorCodeEnum.UnkownError;
            try
            {
                (bool Success, string Message) = EfardaErrors.GetResult(result);

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
                    _errorMsg = $"{result} - {Message}";
                }
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;
                _logger.LogError(ex.Message);
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