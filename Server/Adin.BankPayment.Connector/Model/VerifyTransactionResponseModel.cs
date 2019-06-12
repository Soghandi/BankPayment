namespace Adin.BankPayment.Connector.Model
{
    public class VerifyTransactionResponseModel
    {
        public bool Status { get; set; }

        public byte ErrorCode { get; set; }

        public string Message { get; set; }
    }
}