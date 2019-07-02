using System;
using System.Threading.Tasks;
using Adin.BankPayment.Mellat.Connected_Services.MellayPaymentService;

namespace Adin.BankPayment.Mellat
{
    public class MellatGateway
    {
        private readonly PaymentGatewayClient _client = new PaymentGatewayClient();
        private readonly string _password;
        private readonly string _terminalId;
        private readonly string _userName;


        public MellatGateway(string terminalId, string userName, string password)
        {
            _terminalId = terminalId;
            _password = password;
            _userName = userName;
        }

        public async Task<bpPayRequestResponse> bpPayRequest(int amount, int paymentNumber, string callbackUrl)
        {
            var terminalId = long.Parse(_terminalId);
            var response = await _client.bpPayRequestAsync(terminalId,
                _userName,
                _password,
                paymentNumber,
                amount,
                GetDate(),
                GetTime(),
                paymentNumber.ToString(),
                callbackUrl,
                long.Parse("0")
            );
            return response;
        }

        public async Task<bpInquiryRequestResponse> bpInquiryRequest(long SaleOrderId, long SaleReferenceId)
        {
            var terminalId = long.Parse(_terminalId);
            var response = await _client.bpInquiryRequestAsync(
                long.Parse(_terminalId),
                _userName,
                _password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);

            return response;
        }

        public async Task<bpVerifyRequestResponse> bpVerifyRequest(long SaleOrderId, long SaleReferenceId)
        {
            var terminalId = long.Parse(_terminalId);
            var response = await _client.bpVerifyRequestAsync(
                long.Parse(_terminalId),
                _userName,
                _password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);


            return response;
        }


        public async Task<bpSettleRequestResponse> bpSettleRequest(long SaleOrderId, long SaleReferenceId)
        {
            var terminalId = long.Parse(_terminalId);
            var response = await _client.bpSettleRequestAsync(
                long.Parse(_terminalId),
                _userName,
                _password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);


            return response;
        }

        public async Task<bpReversalRequestResponse> bpReversalRequest(long SaleOrderId, long SaleReferenceId)
        {
            var terminalId = long.Parse(_terminalId);
            var response = await _client.bpReversalRequestAsync(
                long.Parse(_terminalId),
                _userName,
                _password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);

            return response;
        }


        private string GetDate()
        {
            return DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') +
                   DateTime.Now.Day.ToString().PadLeft(2, '0');
        }

        private string GetTime()
        {
            return DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') +
                   DateTime.Now.Second.ToString().PadLeft(2, '0');
        }
    }
}