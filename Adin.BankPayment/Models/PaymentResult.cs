using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Adin.BankPayment.Models
{
    public class PaymentResult
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }
}
