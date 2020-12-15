using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Domain.Model;
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
        private readonly IRepository<ApplicationBank> _applicationBankRepository;
        private string _errorMsg = "";

        public PasargadController(ILogger<PasargadController> logger,
            IRepository<Transaction> transactionRepository,
            IRepository<ApplicationBank> applicationBankRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _applicationBankRepository = applicationBankRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Callback(string token, string secondTrackCode, string InvoiceNumber, string InvoiceDate, string TransactionReferenceID)
        {
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
                var applicationBank = await _applicationBankRepository
                      .GetFirstBy(x => x.ApplicationId == transaction.ApplicationId && x.BankId == transaction.BankId);

                var merchantCode = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "merchantCode");
                var terminalCode = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "terminalCode");
                var privateKey = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "privateKey");

                var pasargadGateway = new PasargadGateway(_logger, merchantCode.ParamValue, terminalCode.ParamValue, privateKey.ParamValue);

                CheckTransactionResultResponseModel CheckTransactionResult;

                if (!string.IsNullOrWhiteSpace(TransactionReferenceID))
                {
                    CheckTransactionResult = await pasargadGateway.CheckTransactionResult(new CheckTransactionResultByReferenceIDRequestModel()
                    {
                        TransactionReferenceID = TransactionReferenceID
                    });
                }
                else
                {
                    CheckTransactionResult = await pasargadGateway.CheckTransactionResult(new CheckTransactionResultByInvoiceNumberRequestModel()
                    {
                        InvoiceDate = transaction.CreationDate.ToString(),
                        InvoiceNumber = transaction.UserTrackCode,
                        MerchantCode = int.Parse(merchantCode.ParamValue),
                        TerminalCode = int.Parse(terminalCode.ParamValue)
                    });
                }

                if (CheckTransactionResult.IsSuccess)
                {
                    var verifyResult = await pasargadGateway.Verify(transaction.UserTrackCode, (int)transaction.Amount, transaction.CreationDate);

                    if (verifyResult.IsSuccess)
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
                        _errorMsg = verifyResult.Message;
                    }
                }
                else
                {
                    _errorMsg = CheckTransactionResult.Message;
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