using System.Threading.Tasks;
using Adin.BankPayment.Parsian.Connected_Services.SaleService;
using Adin.BankPayment.Parsian.Connected_Services.ConfirmService;

namespace Adin.BankPayment.Parsian
{
    public class ParsianGateway
    {
        private readonly SaleServiceSoapClient saleServiceSoapClient =
             new SaleServiceSoapClient(SaleServiceSoapClient.EndpointConfiguration.SaleServiceSoap);

        private readonly ConfirmServiceSoapClient confirmServiceSoapClient =
             new ConfirmServiceSoapClient(ConfirmServiceSoapClient.EndpointConfiguration.ConfirmServiceSoap);

        private readonly string _Pin;

        public ParsianGateway(string Pin)
        {
            _Pin = Pin;
        }

        public Task<SalePaymentRequestResponse> SalePaymentRequestasync(long Amount, long OrderId, string CallBackUrl, string AdditionalData)
        {
            return saleServiceSoapClient.SalePaymentRequestAsync(new ClientSaleRequestData()
            {
                LoginAccount = _Pin,
                Amount = Amount,
                OrderId = OrderId,
                CallBackUrl = CallBackUrl,
                AdditionalData = AdditionalData
            });
        }
        /// <summary>
        /// Verify Transaction Async
        /// </summary>
        /// <param name="Refnum">Bank RefNumber (Bank Token)</param>
        /// <returns></returns>
        public Task<ConfirmPaymentResponse> VerifyTransactionAsync(long Refnum)
        {
            return confirmServiceSoapClient.ConfirmPaymentAsync(new ClientConfirmRequestData()
            {
                LoginAccount = _Pin,
                Token = Refnum
            });
        }
    }
}