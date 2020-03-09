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

        public async Task<bpPayRequestResponse> BpPayRequest(int amount, int paymentNumber, string callbackUrl)
        {
            return await _client.bpPayRequestAsync(long.Parse(_terminalId),
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
        }

        public async Task<bpInquiryRequestResponse> BpInquiryRequest(long SaleOrderId, long SaleReferenceId)
        {
            return await _client.bpInquiryRequestAsync(
                long.Parse(_terminalId),
                _userName,
                _password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);
        }

        public async Task<bpVerifyRequestResponse> BpVerifyRequest(long SaleOrderId, long SaleReferenceId)
        {
            return await _client.bpVerifyRequestAsync(
                long.Parse(_terminalId),
                _userName,
                _password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);
        }


        public async Task<bpSettleRequestResponse> BpSettleRequest(long SaleOrderId, long SaleReferenceId)
        {
            return await _client.bpSettleRequestAsync(
                long.Parse(_terminalId),
                _userName,
                _password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);
        }

        public async Task<bpReversalRequestResponse> BpReversalRequest(long SaleOrderId, long SaleReferenceId)
        {
            return await _client.bpReversalRequestAsync(
                long.Parse(_terminalId),
                _userName,
                _password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);
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