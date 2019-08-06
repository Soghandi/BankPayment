using Adin.BankPayment.Saman.Connected_Services.SamanServiceNew;

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
        
        public double verifyTransaction(string refnum)
        {
            var result1 = _referencePaymentClient.verifyTransactionAsync(refnum, MID).Result;
            return result1;
        }

        public double reverseTransaction(string refnum)
        {
            var result1 = _referencePaymentClient.reverseTransactionAsync(refnum, MID, MID, Password).Result;
            return result1;
        }
    }
}