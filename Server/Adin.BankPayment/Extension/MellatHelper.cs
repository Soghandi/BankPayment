using System;
using System.Linq;
using System.Threading.Tasks;
using Adin.BankPayment.Connector.Enum;
using Adin.BankPayment.Connector.Model;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Mellat;
using Adin.BankPayment.Service;
using Microsoft.Extensions.Logging;

namespace Adin.BankPayment.Extension
{
    public class MellatHelper : IBankHelper
    {
        private readonly IRepository<ApplicationBank> _applicationBankRepository;
        private readonly ILogger _logger;
        private readonly IRepository<Transaction> _transactionRepository;

        public MellatHelper(ILogger logger,
            IRepository<Transaction> transactionRepository,
            IRepository<ApplicationBank> applicationBankRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _applicationBankRepository = applicationBankRepository;
        }

        public async Task<VerifyTransactionResponseModel> VerifyTransaction(Transaction transaction)
        {
            _logger.LogError("Verify Mellat");
            var verifyTransactionResult = new VerifyTransactionResponseModel();

            var applicationBank = await _applicationBankRepository.GetFirstBy(x =>
                x.ApplicationId == transaction.ApplicationId && x.BankId == transaction.BankId);
            var terminalId = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatTerminalId")
                ?.ParamValue;
            var userName = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatUserName")
                ?.ParamValue;
            var password = applicationBank.ApplicationBankParams.FirstOrDefault(x => x.ParamKey == "MellatPassword")
                ?.ParamValue;

            var trans = await _transactionRepository.GetFirstBy(x => x.Id == transaction.Id);
            var saleOrderId = Convert.ToInt64(trans.UserTrackCode);
            var saleReferenceId = Convert.ToInt64(trans.BankTrackCode);

            var mellatGateway = new MellatGateway(terminalId, userName, password);
            var verifyResult = (await mellatGateway.BpVerifyRequest(saleOrderId, saleReferenceId)).Body.@return;
            var message = "تراکنش بازگشت داده شد";
            if (!string.IsNullOrEmpty(verifyResult))
            {
                if (verifyResult == "0")
                {
                    var inquiryResult = (await mellatGateway.BpInquiryRequest(saleOrderId, saleReferenceId)).Body
                        .@return;
                    if (inquiryResult == "0")
                    {
                        //ViewBag.Message = "پرداخت با موفقیت انجام شد.";
                        //ViewBag.SaleReferenceId = SaleReferenceId;
                        // پرداخت نهایی
                        // تایید پرداخت
                        var settleResult = (await mellatGateway.BpSettleRequest(saleOrderId, saleReferenceId)).Body
                            .@return;
                        if (settleResult != null)
                        {
                            if (settleResult == "0" || settleResult == "45")
                            {
                                //تراکنش تایید و ستل شده است 
                                _logger.LogError("Verify Done!!!!!");

                                message = "بانک صحت رسيد ديجيتالي شما را تصديق نمود. فرايند خريد تکميل گشت";
                                message += "<br/>" + " شماره رسید : " + transaction.BankTrackCode;
                                transaction.Status = (byte) TransactionStatusEnum.Success;
                                transaction.ModifiedOn = DateTime.Now;
                                transaction.ModifiedBy = 1;
                                await _transactionRepository.Update(transaction);

                                verifyTransactionResult.Status = true;
                                verifyTransactionResult.ErrorCode = (byte) ErrorCodeEnum.NoError;
                                verifyTransactionResult.Message = message;
                                _logger.LogError("Mellat Verify Done!");
                                return verifyTransactionResult;
                            }

                            //تراکنش تایید شده ولی ستل نشده است                                
                            _logger.LogError("Verify Done!!!!!");

                            message = "بانک صحت رسيد ديجيتالي شما را تصديق نمود. فرايند خريد تکميل گشت";
                            message += "<br/>" + " شماره رسید : " + transaction.BankTrackCode;
                            transaction.Status = (byte) TransactionStatusEnum.WaitingForSettle;
                            transaction.ModifiedOn = DateTime.Now;
                            transaction.ModifiedBy = 1;
                            await _transactionRepository.Update(transaction);

                            verifyTransactionResult.Status = true;
                            verifyTransactionResult.ErrorCode = (byte) ErrorCodeEnum.NoError;
                            verifyTransactionResult.Message = message;
                            _logger.LogError("Mellat Verify Done!");
                            return verifyTransactionResult;
                        }
                    }
                    else
                    {
                        //string Rvresult;
                        //عملیات برگشت دادن مبلغ
                        var result = (await mellatGateway.BpReversalRequest(saleOrderId, saleReferenceId)).Body.@return;
                        message = "تراکنش بازگشت داده شد";

                        _logger.LogError("resultcode" + result);
                        transaction.Status = (byte) TransactionStatusEnum.ErrorOnVerify;
                        transaction.BankErrorCode = Convert.ToInt32(result);
                        transaction.BankErrorMessage = MellatResult(result);
                        transaction.ModifiedOn = DateTime.Now;
                        transaction.ModifiedBy = 1;
                        await _transactionRepository.Update(transaction);

                        verifyTransactionResult.Status = false;
                        verifyTransactionResult.ErrorCode = (byte) ErrorCodeEnum.VerifyError;
                        verifyTransactionResult.Message = message;
                        _logger.LogError("Mellat Verify reverse!");
                        return verifyTransactionResult;
                    }
                }
                else
                {
                    //ViewBag.Message = MellatHelper.MellatResult(Result);
                    //ViewBag.SaleReferenceId = "**************";                  
                    message = "تراکنش بازگشت داده شد";

                    _logger.LogError("errr1");
                    transaction.Status = (byte) TransactionStatusEnum.ErrorOnVerify;
                    transaction.BankErrorCode = (byte) ErrorCodeEnum.VerifyError;
                    transaction.BankErrorMessage = message;
                    transaction.ModifiedOn = DateTime.Now;
                    transaction.ModifiedBy = 1;
                    await _transactionRepository.Update(transaction);

                    verifyTransactionResult.Status = false;
                    verifyTransactionResult.ErrorCode = (byte) ErrorCodeEnum.VerifyError;
                    verifyTransactionResult.Message = message;
                    _logger.LogError("Mellat Verify error1!");
                    return verifyTransactionResult;
                }
            }
            else
            {
                //ViewBag.Message = "شماره رسید قابل قبول نیست";
                //ViewBag.SaleReferenceId = "**************";

                _logger.LogError("err2");
                transaction.Status = (byte) TransactionStatusEnum.ErrorOnVerify;
                transaction.BankErrorCode = (byte) ErrorCodeEnum.VerifyError;
                transaction.BankErrorMessage = message;
                transaction.ModifiedOn = DateTime.Now;
                transaction.ModifiedBy = 1;
                await _transactionRepository.Update(transaction);

                verifyTransactionResult.Status = false;
                verifyTransactionResult.ErrorCode = (byte) ErrorCodeEnum.VerifyError;
                verifyTransactionResult.Message = message;
                _logger.LogError("Mellat Verify error2!");
                return verifyTransactionResult;
            }

            return verifyTransactionResult;
        }


        public static string MellatResult(string id)
        {
            string result;
            switch (id)
            {
                case "-100":
                    result = "پرداخت لغو شده";
                    break;
                case "0":
                    result = "تراكنش با موفقيت انجام شد";
                    break;
                case "11":
                    result = "شماره كارت نامعتبر است ";
                    break;
                case "12":
                    result = "موجودي كافي نيست ";
                    break;
                case "13":
                    result = "رمز نادرست است ";
                    break;
                case "14":
                    result = "تعداد دفعات وارد كردن رمز بيش از حد مجاز است ";
                    break;
                case "15":
                    result = "كارت نامعتبر است ";
                    break;
                case "16":
                    result = "دفعات برداشت وجه بيش از حد مجاز است ";
                    break;
                case "17":
                    result = "كاربر از انجام تراكنش منصرف شده است ";
                    break;
                case "18":
                    result = "تاريخ انقضاي كارت گذشته است ";
                    break;
                case "19":
                    result = "مبلغ برداشت وجه بيش از حد مجاز است ";
                    break;
                case "111":
                    result = "صادر كننده كارت نامعتبر است ";
                    break;
                case "112":
                    result = "خطاي سوييچ صادر كننده كارت ";
                    break;
                case "113":
                    result = "پاسخي از صادر كننده كارت دريافت نشد ";
                    break;
                case "114":
                    result = "دارنده كارت مجاز به انجام اين تراكنش نيست";
                    break;
                case "21":
                    result = "پذيرنده نامعتبر است ";
                    break;
                case "23":
                    result = "خطاي امنيتي رخ داده است ";
                    break;
                case "24":
                    result = "اطلاعات كاربري پذيرنده نامعتبر است ";
                    break;
                case "25":
                    result = "مبلغ نامعتبر است ";
                    break;
                case "31":
                    result = "پاسخ نامعتبر است ";
                    break;
                case "32":
                    result = "فرمت اطلاعات وارد شده صحيح نمي باشد ";
                    break;
                case "33":
                    result = "حساب نامعتبر است ";
                    break;
                case "34":
                    result = "خطاي سيستمي ";
                    break;
                case "35":
                    result = "تاريخ نامعتبر است ";
                    break;
                case "41":
                    result = "شماره درخواست تكراري است ، دوباره تلاش کنید";
                    break;
                case "42":
                    result = "يافت نشد  Sale تراكنش";
                    break;
                case "43":
                    result = "داده شده است  Verify قبلا درخواست";
                    break;
                case "44":
                    result = "يافت نشد  Verfiy درخواست";
                    break;
                case "45":
                    result = "شده است  Settle تراكنش";
                    break;
                case "46":
                    result = "نشده است  Settle تراكنش";
                    break;
                case "47":
                    result = "يافت نشد  Settle تراكنش";
                    break;
                case "48":
                    result = "شده است  Reverse تراكنش";
                    break;
                case "49":
                    result = "يافت نشد  Refund تراكنش";
                    break;
                case "412":
                    result = "شناسه قبض نادرست است ";
                    break;
                case "413":
                    result = "شناسه پرداخت نادرست است ";
                    break;
                case "414":
                    result = "سازمان صادر كننده قبض نامعتبر است ";
                    break;
                case "415":
                    result = "زمان جلسه كاري به پايان رسيده است ";
                    break;
                case "416":
                    result = "خطا در ثبت اطلاعات ";
                    break;
                case "417":
                    result = "شناسه پرداخت كننده نامعتبر است ";
                    break;
                case "418":
                    result = "اشكال در تعريف اطلاعات مشتري ";
                    break;
                case "419":
                    result = "تعداد دفعات ورود اطلاعات از حد مجاز گذشته است ";
                    break;
                case "421":
                    result = "نامعتبر است  IP";
                    break;
                case "51":
                    result = "تراكنش تكراري است ";
                    break;
                case "54":
                    result = "تراكنش مرجع موجود نيست ";
                    break;
                case "55":
                    result = "تراكنش نامعتبر است ";
                    break;
                case "61":
                    result = "خطا در واريز ";
                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }


        public static ErrorCodeEnum ErrorResult(string id)
        {
            switch (id)
            {
                case "-100":

                    //result = "پرداخت لغو شده";
                    return ErrorCodeEnum.CanceledByUser;
                case "0":
                    //result = "تراكنش با موفقيت انجام شد";
                    return ErrorCodeEnum.NoError;
                case "11":
                    return ErrorCodeEnum.InvalidCardNumber;
                //result = "شماره كارت نامعتبر است ";                    
                case "12":
                    return ErrorCodeEnum.NoSufficientFunds;
                //result = "موجودي كافي نيست ";                    
                case "13":
                    return ErrorCodeEnum.InvalidPassword;
                //result = "رمز نادرست است ";
                //break;
                case "14":
                    return ErrorCodeEnum.InvalidPassword;

                //result = "تعداد دفعات وارد كردن رمز بيش از حد مجاز است ";
                //break;
                case "15":
                    return ErrorCodeEnum.InvalidCardNumber;
                //result = "كارت نامعتبر است ";
                //break;
                case "16":
                    return ErrorCodeEnum.ExceedsWithdrawalAmountLimit;

                //result = "دفعات برداشت وجه بيش از حد مجاز است ";
                //break;
                case "17":
                    return ErrorCodeEnum.CanceledByUser;

                //result = "كاربر از انجام تراكنش منصرف شده است ";
                //break;
                case "18":
                    return ErrorCodeEnum.ExpiredCardPickUp;

                //   result = "تاريخ انقضاي كارت گذشته است ";
                //  break;
                case "19":
                    return ErrorCodeEnum.ExceedsWithdrawalAmountLimit;

                //result = "مبلغ برداشت وجه بيش از حد مجاز است ";
                //break;
                case "111":
                    return ErrorCodeEnum.BankIssuerIsInvalid;

                //                    result = "صادر كننده كارت نامعتبر است ";
                //                   break;
                case "112":
                    return ErrorCodeEnum.UnkownError;

                //  result = "خطاي سوييچ صادر كننده كارت ";
                // break;
                case "113":
                    return ErrorCodeEnum.UnkownError;

                //result = "پاسخي از صادر كننده كارت دريافت نشد ";
                //break;
                case "114":
                    return ErrorCodeEnum.AllowablePINTriesExceededPickUp;


                //result = "دارنده كارت مجاز به انجام اين تراكنش نيست";
                //break;
                case "21":
                    return ErrorCodeEnum.UnkownError;

                //result = "پذيرنده نامعتبر است ";
                //break;
                case "23":
                    return ErrorCodeEnum.UnkownError;

                //result = "خطاي امنيتي رخ داده است ";
                //break;
                case "24":
                    return ErrorCodeEnum.UnkownError;

                //result = "اطلاعات كاربري پذيرنده نامعتبر است ";
                //break;
                case "25":
                    return ErrorCodeEnum.UnkownError;

                //result = "مبلغ نامعتبر است ";
                //break;
                case "31":
                    return ErrorCodeEnum.UnkownError;

                //result = "پاسخ نامعتبر است ";
                //break;
                case "32":
                    return ErrorCodeEnum.UnkownError;

                //result = "فرمت اطلاعات وارد شده صحيح نمي باشد ";
                //break;
                case "33":
                    return ErrorCodeEnum.UnkownError;

                //result = "حساب نامعتبر است ";
                //break;
                case "34":
                    return ErrorCodeEnum.InternalError;

                //result = "خطاي سيستمي ";
                //break;
                case "35":
                    return ErrorCodeEnum.InvalidDate;

                //result = "تاريخ نامعتبر است ";
                //break;
                case "41":
                    return ErrorCodeEnum.UnkownError;

                //result = "شماره درخواست تكراري است ، دوباره تلاش کنید";
                //break;
                case "42":
                    return ErrorCodeEnum.UnkownError;

                //result = "يافت نشد  Sale تراكنش";
                //break;
                case "43":
                    return ErrorCodeEnum.AlreadyVerified;

                //result = "داده شده است  Verify قبلا درخواست";
                //break;
                case "44":
                    return ErrorCodeEnum.UnkownError;

                //result = "يافت نشد  Verfiy درخواست";
                //break;
                case "45":
                    return ErrorCodeEnum.UnkownError;

                //result = "شده است  Settle تراكنش";
                //break;
                case "46":
                    return ErrorCodeEnum.UnkownError;

                //result = "نشده است  Settle تراكنش";
                //break;
                case "47":
                    return ErrorCodeEnum.UnkownError;

                //result = "يافت نشد  Settle تراكنش";
                //break;
                case "48":
                    return ErrorCodeEnum.UnkownError;

                //result = "شده است  Reverse تراكنش";
                //break;
                case "49":
                    return ErrorCodeEnum.UnkownError;

                //result = "يافت نشد  Refund تراكنش";
                //break;
                case "412":
                    return ErrorCodeEnum.InvalidBillCode;

                //result = "شناسه قبض نادرست است ";
                //break;
                case "413":
                    return ErrorCodeEnum.InvalidPaymentCode;

                //result = "شناسه پرداخت نادرست است ";
                //break;
                case "414":
                    return ErrorCodeEnum.NoSuchIssuer;

                //result = "سازمان صادر كننده قبض نامعتبر است ";
                //break;
                case "415":
                    return ErrorCodeEnum.ResponseReceivedTooLate;

                //result = "زمان جلسه كاري به پايان رسيده است ";
                //break;
                case "416":
                    return ErrorCodeEnum.UnkownError;

                //result = "خطا در ثبت اطلاعات ";
                //break;
                case "417":
                    return ErrorCodeEnum.UnkownError;

                //result = "شناسه پرداخت كننده نامعتبر است ";
                //break;
                case "418":
                    return ErrorCodeEnum.UnkownError;

                //result = "اشكال در تعريف اطلاعات مشتري ";
                //break;
                case "419":
                    return ErrorCodeEnum.UnkownError;

                //result = "تعداد دفعات ورود اطلاعات از حد مجاز گذشته است ";
                //break;
                case "421":

                    return ErrorCodeEnum.InvalidIP;

                //result = "نامعتبر است  IP";
                //break;
                case "51":
                    return ErrorCodeEnum.OperationAlreadyDone;

                //result = "تراكنش تكراري است ";
                //break;
                case "54":
                    return ErrorCodeEnum.UnkownError;

                //result = "تراكنش مرجع موجود نيست ";
                //break;
                case "55":
                    return ErrorCodeEnum.InvalidTransaction;

                //result = "تراكنش نامعتبر است ";
                //break;
                case "61":
                    return ErrorCodeEnum.UnkownError;

                //result = "خطا در واريز ";
                //break;
                default:
                    return ErrorCodeEnum.UnkownError;
                //result = string.Empty;
                //break;
            }
        }
    }
}