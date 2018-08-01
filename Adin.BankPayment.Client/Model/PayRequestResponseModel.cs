using Adin.BankPayment.Client.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adin.BankPayment.Client
{
    public class PayRequestResponseModel
    {        

        public Guid Code { get; set; }

        public string Url { get; set; }

    }
}
