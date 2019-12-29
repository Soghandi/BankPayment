using System.Threading.Tasks;
using Adin.BankPayment.Parsian.Connected_Services.ParsianPaymentService;

namespace Adin.BankPayment.Parsian
{
    public class ParsianGateway
    {
        private readonly EShopServiceSoapClient client =
            new EShopServiceSoapClient(EShopServiceSoapClient.EndpointConfiguration.EShopServiceSoap);

        private readonly string Pin;

        private string Password;

        public ParsianGateway(string pin, string password = "")
        {
            Pin = pin;
            Password = password;
        }

        public async Task<PinPaymentRequestResponse> PinPaymentRequest(int amount, int paymentNumber,
            string callbackUrl)
        {
            long authority = 0;
            byte status = 1;
            var response =
                await client.PinPaymentRequestAsync(Pin, amount, paymentNumber, callbackUrl, authority, status);
            return response;
        }

        //public double verifyTransaction(string refnum)
        //{
        //    var result1 = referencePaymentClient.verifyTransactionAsync(refnum, MID).Result;
        //    return result1;
        //}

        //public double reverseTransaction(string refnum)
        //{        
        //    var result1 = referencePaymentClient.reverseTransactionAsync(refnum, MID, MID, Password).Result;
        //    return result1;
        //}
    }
}