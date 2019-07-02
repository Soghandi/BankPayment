using System.Threading.Tasks;
using Adin.BankPayment.Connector.Model;
using Adin.BankPayment.Domain.Model;

namespace Adin.BankPayment.Extension
{
    public interface IBankHelper
    {
        Task<VerifyTransactionResponseModel> VerifyTransaction(Transaction transaction);
    }
}