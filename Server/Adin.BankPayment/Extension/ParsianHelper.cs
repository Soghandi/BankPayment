using System.Threading.Tasks;
using Adin.BankPayment.Connector.Model;
using Adin.BankPayment.Domain.Model;

namespace Adin.BankPayment.Extension
{
    public class ParsianHelper : IBankHelper
    {
        public async Task<VerifyTransactionResponseModel> VerifyTransaction(Transaction transaction)
        {
            var verifyTransactionResult = new VerifyTransactionResponseModel();

            return verifyTransactionResult;
        }
    }
}