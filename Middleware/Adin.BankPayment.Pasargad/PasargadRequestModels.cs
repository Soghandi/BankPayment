namespace Adin.BankPayment.Pasargad
{
    public class GetTokenRequestModel
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string TerminalCode { get; set; }
        public string MerchantCode { get; set; }
        public int Amount { get; set; }
        public string RedirectAddress { get; set; }
        public string Timestamp { get; set; }
        public int Action { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string MerchantName { get; set; }
        public string ChargeCompany { get; set; }
        public string TopupNumber { get; set; }
    }

    public class CheckTransactionResultByReferenceIDRequestModel
    {
        public string TransactionReferenceID { get; set; }
    }

    public class CheckTransactionResultByInvoiceNumberRequestModel
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public int MerchantCode { get; set; }
        public int TerminalCode { get; set; }
    }

    //public class RefundPaymentRequestModel
    //{
    //    public string InvoiceNumber { get; set; }
    //    public string InvoiceDate { get; set; }
    //    public string TerminalCode { get; set; }
    //    public string MerchantCode { get; set; }
    //    public int Amount { get; set; }
    //    public int Action { get; set; }
    //    public string Timestamp { get; set; }
    //}

    public class VerifyPaymentRequestModel
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string TerminalCode { get; set; }
        public string MerchantCode { get; set; }
        public int Amount { get; set; }
        public string Timestamp { get; set; }
    }
}