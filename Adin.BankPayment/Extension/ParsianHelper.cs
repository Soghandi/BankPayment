
using Adin.BankPayment.Connector.Model;
using Adin.BankPayment.Domain.Model;
using System.Threading.Tasks;

namespace Adin.BankPayment.Extension
{

    public class ParsianHelper : BankHelper
    {
        public async Task<VerifyTransactionResponseModel> VerifyTransaction(Transaction transaction)
        {
            VerifyTransactionResponseModel verifyTransactionResult = new VerifyTransactionResponseModel();

            return verifyTransactionResult;
        }
    }
}
