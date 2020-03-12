namespace Adin.BankPayment.Connector.Enum
{
    public enum ErrorCodeEnum
    {
        NoError = 0,
        CanceledByUser = 1,
        InvalidParameters = 2,
        InvalidTransaction = 3,
        InvalidCardNumber = 4,
        NoSuchIssuer = 5,
        ExpiredCardPickUp = 6,
        AllowablePINTriesExceededPickUp = 7,
        IncorrectPIN = 8,
        ExceedsWithdrawalAmountLimit = 8,
        TransactionCannotBeCompleted = 9,
        ResponseReceivedTooLate = 10,
        SuspectedFraudPickUp = 11,
        NoSufficientFunds = 12,
        BankServerIsDown = 13,
        InvalidAmount = 14,
        UnkownError = 15,
        InvalidPassword = 16,
        BankIssuerIsInvalid = 17,
        InvalidIP = 18,
        InvalidBillCode = 19,
        InvalidPaymentCode = 20,
        AlreadyVerified = 21,
        InvalidDate = 22,
        InvalidTime = 23,
        InternalError = 24,
        VerifyError = 100,
        InvalidPin = 200, //پین فروشنده درست نیست :
        OperationAlreadyDone = 201, //عملیات قبلا با موفقیت انجام شده است
        UserTrackCodeIsInvalid = 202 //شماره تراكنش فروشنده درست نیست        
    }
}