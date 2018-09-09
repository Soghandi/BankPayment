using System;
using System.Collections.Generic;
using System.Text;

namespace Adin.BankPayment.Connector.Model
{
    public class CancelPaymentResponseModel
    {
        public bool Status { get; set; }

        public byte ErrorCode { get; set; }

        public string Message { get; set; }

    }
}
