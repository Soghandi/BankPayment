using System;

namespace Adin.BankPayment.Connector.Model
{
    public class PayRequestResponseModel
    {
        public Guid Code { get; set; }

        public string Url { get; set; }
    }
}