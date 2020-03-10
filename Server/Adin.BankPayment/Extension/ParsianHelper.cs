using System.Linq;
using System.Threading.Tasks;
using Adin.BankPayment.Connector.Model;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Service;
using Microsoft.Extensions.Logging;
using Adin.BankPayment.Parsian;
using System;
using Adin.BankPayment.Connector.Enum;

namespace Adin.BankPayment.Extension
{
    public class ParsianHelper : IBankHelper
    {
        private readonly IRepository<ApplicationBank> _applicationBankRepository;
        private readonly ILogger _logger;
        private readonly IRepository<Transaction> _transactionRepository;

        public ParsianHelper(ILogger logger,
            IRepository<Transaction> transactionRepository,
            IRepository<ApplicationBank> applicationBankRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _applicationBankRepository = applicationBankRepository;
        }

        public async Task<VerifyTransactionResponseModel> VerifyTransaction(Transaction transaction)
        {
            var verifyTransactionResult = new VerifyTransactionResponseModel();

            var applicationBanks = await _applicationBankRepository.GetFirstBy(x =>
                x.BankId == transaction.BankId && x.ApplicationId == transaction.ApplicationId);
            var ParsianParams = applicationBanks.ApplicationBankParams.ToList();
            var midParam = ParsianParams.FirstOrDefault(x => x.ParamKey == "MID");

            if (midParam == null)
                return verifyTransactionResult;

            var ParsianGateway = new ParsianGateway(midParam.ParamValue);

            var result = await ParsianGateway.VerifyTransactionAsync(Convert.ToInt64(transaction.BankTrackCode));

            if (result.Body.ConfirmPaymentResult.Status == 0 && result.Body.ConfirmPaymentResult.Token > 0)
            {
                _logger.LogDebug("Verify Done");
                var message = "بانک صحت رسید دیجیتالی شما را تصدیق نمود. فرایند خرید تکمیل گشت";
                message += "<br/>" + " شماره رسید : " + transaction.BankTrackCode;
                transaction.Status = (byte)TransactionStatusEnum.Success;
                transaction.ModifiedOn = DateTime.Now;
                transaction.ModifiedBy = 1;
                await _transactionRepository.Update(transaction);

                verifyTransactionResult.Status = true;
                verifyTransactionResult.ErrorCode = (byte)ErrorCodeEnum.NoError;
                verifyTransactionResult.Message = message;

                return verifyTransactionResult;
            }

            var (Success, Message) = ParsianErrors.GetResult(result.Body.ConfirmPaymentResult.Status);
            _logger.LogDebug("Result code: " + result);
            transaction.Status = (byte)TransactionStatusEnum.ErrorOnVerify;
            transaction.BankErrorCode = Convert.ToInt32(result);
            transaction.BankErrorMessage = Message;
            transaction.ModifiedOn = DateTime.Now;
            transaction.ModifiedBy = 1;
            await _transactionRepository.Update(transaction);

            verifyTransactionResult.Status = false;
            verifyTransactionResult.ErrorCode = (byte)ErrorCodeEnum.VerifyError;
            verifyTransactionResult.Message = Message;

            return verifyTransactionResult;
        }
    }
}