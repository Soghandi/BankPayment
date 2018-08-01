using Adin.BankPayment.Connector.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adin.BankPayment.Connector.Model
{
    public class OutputModel<T> 
    {
        public ApiStatusCodeEnum Status { get; set; }

        public T Body { get; set; }

    }
}
