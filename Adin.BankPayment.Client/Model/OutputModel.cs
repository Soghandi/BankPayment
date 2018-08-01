using Adin.BankPayment.Client.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adin.BankPayment.Client
{
    public class OutputModel<T> 
    {
        public ApiStatusCodeEnum Status { get; set; }

        public T Body { get; set; }

    }
}
