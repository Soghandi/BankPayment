using System.Collections.Generic;

namespace Adin.BankPayment.Saman
{

    public class SamanGateway
    {

        public SamanGateway(string samanMID,string password="")
        {
            MID = samanMID;
            Password = password;
        }

        private string MID;
        private string Password;
        private string redirectUrl;
        private SamanService.PaymentIFBindingSoapClient referencePaymentClient = new SamanService.PaymentIFBindingSoapClient(SamanService.PaymentIFBindingSoapClient.EndpointConfiguration.PaymentIFBindingSoap);



        public double verifyTransaction(string refnum)
        {
            var result1 = referencePaymentClient.verifyTransactionAsync(refnum, MID).Result;
            return result1;
        }

        public double reverseTransaction(string refnum)
        {
            var result1 = referencePaymentClient.reverseTransactionAsync(refnum, MID,MID,Password).Result;
            return result1;
        }
    }
}
