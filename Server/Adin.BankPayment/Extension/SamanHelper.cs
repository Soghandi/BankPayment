
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Connector.Model;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Adin.BankPayment.Extension
{
    public class SamanHelper : BankHelper
    {
        private readonly ILogger _logger;
        private IRepository<Transaction> _transactionRepository;
        private IRepository<Application> _applicationRepository;
        private IRepository<Bank> _bankRepository;
        private IRepository<ApplicationBank> _applicationBankRepository;
        public SamanHelper(ILogger logger,
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
        
        public async Task<VerifyTransactionResponseModel> VerifyTransaction(Transaction transaction)
        {
            VerifyTransactionResponseModel verifyTransactionResult = new VerifyTransactionResponseModel();


            ///WebService Instance

            var applicationBanks = await _applicationBankRepository.GetFirstBy(x => x.BankId == transaction.BankId && x.ApplicationId == transaction.ApplicationId);
            var samanParams = applicationBanks.ApplicationBankParams.ToList();
            var midParam = samanParams.FirstOrDefault(x => x.ParamKey == "MID");
            //var passwordParam = samanParams.FirstOrDefault(x => x.ParamKey == "Password");
            Saman.SamanGateway samanGateway = new Saman.SamanGateway(midParam.ParamValue);

            string message = "";
            var result = samanGateway.verifyTransaction(transaction.BankTrackCode);

            if (result > 0)
            {

                //if (result == (double)transaction.Amount) // چک کردن مبلغ بازگشتی از سرویس با مبلغ تراکنش
                //{

                _logger.LogDebug("Verify Done");
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
            else
            {
                var _errorMsg = TransactionChecking((int)result);
                _logger.LogDebug("resultcode" + result);
                transaction.Status = (byte)TransactionStatusEnum.ErrorOnVerify;
                transaction.BankErrorCode = Convert.ToInt32(result);
                transaction.BankErrorMessage = _errorMsg;
                transaction.ModifiedOn = DateTime.Now;
                transaction.ModifiedBy = 1;
                await _transactionRepository.Update(transaction);

                verifyTransactionResult.Status = false;
                verifyTransactionResult.ErrorCode = (byte)ErrorCodeEnum.VerifyError;
                verifyTransactionResult.Message = _errorMsg;
                return verifyTransactionResult;
               
            }
        }


        private string TransactionChecking(int i)
        {
            bool isError = false;
            string errorMsg = "";
            switch (i)
            {

                case -1:		//TP_ERROR
                    isError = true;
                    errorMsg = "بروز خطا درهنگام بررسي صحت رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-1";
                    break;
                case -2:		//ACCOUNTS_DONT_MATCH
                    isError = true;
                    errorMsg = "بروز خطا در هنگام تاييد رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-2";
                    break;
                case -3:		//BAD_INPUT
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-3";
                    break;
                case -4:		//INVALID_PASSWORD_OR_ACCOUNT
                    isError = true;
                    errorMsg = "خطاي دروني سيستم درهنگام بررسي صحت رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-4";
                    break;
                case -5:		//DATABASE_EXCEPTION
                    isError = true;
                    errorMsg = "خطاي دروني سيستم درهنگام بررسي صحت رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-5";
                    break;
                case -7:		//ERROR_STR_NULL
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-7";
                    break;
                case -8:		//ERROR_STR_TOO_LONG
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-8";
                    break;
                case -9:		//ERROR_STR_NOT_AL_NUM
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-9";
                    break;
                case -10:	//ERROR_STR_NOT_BASE64
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-10";
                    break;
                case -11:	//ERROR_STR_TOO_SHORT
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-11";
                    break;
                case -12:		//ERROR_STR_NULL
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-12";
                    break;
                case -13:		//ERROR IN AMOUNT OF REVERS TRANSACTION AMOUNT
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-13";
                    break;
                case -14:	//INVALID TRANSACTION
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-14";
                    break;
                case -15:	//RETERNED AMOUNT IS WRONG
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-15";
                    break;
                case -16:	//INTERNAL ERROR
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-16";
                    break;
                case -17:	// REVERS TRANSACTIN FROM OTHER BANK
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-17";
                    break;
                case -18:	//INVALID IP
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-18";
                    break;

            }
            return errorMsg;
        }
    }
}
