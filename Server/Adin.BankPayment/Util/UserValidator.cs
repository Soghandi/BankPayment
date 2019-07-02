using System.Linq;
using Adin.BankPayment.Domain.Context;
using Adin.BankPayment.Domain.Model;

namespace Adin.BankPayment.Util
{
    public class UserValidator
    {
        public Application Validate(string publicKey)
        {
            using (var context = new BankPaymentContext())
            {
                var app = context.Applications.FirstOrDefault(x => x.PublicKey == publicKey);
                if (app == null) return null;
                return app;
            }
        }
    }
}