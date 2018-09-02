namespace Adin.BankPayment.Connector.Enum
{
    public enum TransactionStatusEnum
    {
        Initial = 0,
        Success = 1,
        Failed = 2,
        BankOk = 3,
        ErrorOnVerify = 4,
        ReverseTransaction = 5,
        WaitingForSettle = 6
    }
}