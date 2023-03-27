namespace Adin.BankPayment.Pasargad
{
    public class GetTokenResponseModel
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// if IsSuccess is true ...
        /// </summary>
        public string Token { get; set; }
    }

    public class VerifyResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string MaskedCardNumber { get; set; }
        public string HashedCardNumber { get; set; }
        public string ShaparakRefNumber { get; set; }
    }

    public class CheckTransactionResultResponseModel
    {
        public int TraceNumber { get; set; }
        public long ReferenceNumber { get; set; }
        public string TransactionDate { get; set; }
        public string Action { get; set; }
        public string TransactionReferenceID { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public int MerchantCode { get; set; }
        public int TerminalCode { get; set; }
        public int Amount { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}