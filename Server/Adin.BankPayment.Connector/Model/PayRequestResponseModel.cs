using Adin.BankPayment.Connector.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adin.BankPayment.Connector.Model
{
    public class PayRequestResponseModel
    {        

        public Guid Code { get; set; }

        public string Url { get; set; }

    }
}
