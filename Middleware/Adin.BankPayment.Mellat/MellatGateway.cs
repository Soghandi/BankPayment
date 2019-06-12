using System;
using System.Threading.Tasks;
using MellayPaymentService;

namespace Adin.BankPayment.Mellat
{
    public class MellatGateway
    {
        private readonly PaymentGatewayClient client = new PaymentGatewayClient();
        private readonly string Password;
        private readonly string TerminalId;
        private readonly string UserName;


        public MellatGateway(string terminalId, string userName, string password)
        {
            TerminalId = terminalId;
            Password = password;
            UserName = userName;
        }

        public async Task<bpPayRequestResponse> bpPayRequest(int amount, int paymentNumber, string callbackUrl)
        {
            var terminalId = long.Parse(TerminalId);
            var response = await client.bpPayRequestAsync(terminalId,
                UserName,
                Password,
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
            var terminalId = long.Parse(TerminalId);
            var response = await client.bpInquiryRequestAsync(
                long.Parse(TerminalId),
                UserName,
                Password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);

            return response;
        }

        public async Task<bpVerifyRequestResponse> bpVerifyRequest(long SaleOrderId, long SaleReferenceId)
        {
            var terminalId = long.Parse(TerminalId);
            var response = await client.bpVerifyRequestAsync(
                long.Parse(TerminalId),
                UserName,
                Password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);


            return response;
        }


        public async Task<bpSettleRequestResponse> bpSettleRequest(long SaleOrderId, long SaleReferenceId)
        {
            var terminalId = long.Parse(TerminalId);
            var response = await client.bpSettleRequestAsync(
                long.Parse(TerminalId),
                UserName,
                Password,
                SaleOrderId,
                SaleOrderId,
                SaleReferenceId);


            return response;
        }

        public async Task<bpReversalRequestResponse> bpReversalRequest(long SaleOrderId, long SaleReferenceId)
        {
            var terminalId = long.Parse(TerminalId);
            var response = await client.bpReversalRequestAsync(
                long.Parse(TerminalId),
                UserName,
                Password,
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