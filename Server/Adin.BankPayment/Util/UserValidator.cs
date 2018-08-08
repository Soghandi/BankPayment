using Adin.BankPayment.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Adin.BankPayment.Util
{
    public class UserValidator
    {
        public Application Validate(string publicKey)
        {
            using (var context = new BankPayment.Domain.Context.BankPaymentContext())
            {
                var app = context.Applications.FirstOrDefault(x => x.PublicKey == publicKey);
                if (app == null) return null;
                else
                {
                    return app;
                }
            }
        }
    }
}
