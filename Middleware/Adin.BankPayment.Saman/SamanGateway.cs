using Adin.BankPayment.Saman.Connected_Services.SamanServiceNew;
using System.Threading.Tasks;

namespace Adin.BankPayment.Saman
{
    public class SamanGateway
    {
        private readonly string MID;
        private readonly string Password;

        private readonly PaymentIFBindingSoapClient _referencePaymentClient =
            new PaymentIFBindingSoapClient(PaymentIFBindingSoapClient.EndpointConfiguration.PaymentIFBindingSoap);

        public SamanGateway(string samanMID, string password = "")
        {
            MID = samanMID;
            Password = password;
        }

        public Task<double> VerifyTransactionAsync(string Refnum)
        {
            return _referencePaymentClient.verifyTransactionAsync(Refnum, MID);
        }

        public Task<double> ReverseTransactionAsync(string Refnum)
        {
            return _referencePaymentClient.reverseTransactionAsync(Refnum, MID, MID, Password);
        }
    }
}