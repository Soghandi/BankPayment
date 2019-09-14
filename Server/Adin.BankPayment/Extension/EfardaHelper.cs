using System;
using System.Linq;
using System.Threading.Tasks;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Connector.Model;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Efarda;
using Adin.BankPayment.Service;
using Microsoft.Extensions.Logging;

namespace Adin.BankPayment.Extension
{
    public class EfardaHelper : IBankHelper
    {
        private readonly IRepository<ApplicationBank> _applicationBankRepository;
        private readonly ILogger _logger;
        private readonly IRepository<Transaction> _transactionRepository;

        public EfardaHelper(ILogger logger,
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

            var applicationBank = await _applicationBankRepository.GetFirstBy(x =>
                x.ApplicationId == transaction.ApplicationId && x.BankId == transaction.BankId);
            
            var EfardaServiceId =
                      applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "serviceId");
            var EfardaUsername =
                applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "userName");
            var EfardaPassword =
                applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "password");

            var trans = await _transactionRepository.GetFirstBy(x => x.Id == transaction.Id);
            var saleOrderId = Convert.ToInt64(trans.UserTrackCode);
            var saleReferenceId = Convert.ToInt64(trans.BankTrackCode);

            EfardaGateway efardaGateway = new EfardaGateway(_logger, EfardaUsername.ParamValue, EfardaPassword.ParamValue, EfardaServiceId.ParamValue);
            var traceNumber = await efardaGateway.Verify(transaction.BankTrackCode);


            var message = "تراکنش بازگشت داده شد";

            if (traceNumber)
            {
                message = "بانک صحت رسيد ديجيتالي شما را تصديق نمود. فرايند خريد تکميل گشت";
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

            transaction.Status = (byte)TransactionStatusEnum.ErrorOnVerify;
            transaction.BankErrorCode = (byte)ErrorCodeEnum.VerifyError;
            transaction.BankErrorMessage = message;
            transaction.ModifiedOn = DateTime.Now;
            transaction.ModifiedBy = 1;
            await _transactionRepository.Update(transaction);

            verifyTransactionResult.Status = false;
            verifyTransactionResult.ErrorCode = (byte)ErrorCodeEnum.VerifyError;
            verifyTransactionResult.Message = message;
            _logger.LogError($"EfardaHelper - VerifyTransaction - Failed - transactionId: {transaction.Id}");
            return verifyTransactionResult;
        }
    }
}