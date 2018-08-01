using MellayPaymentService;
using System;
using System.Threading.Tasks;

namespace Adin.BankPayment.Mellat
{
    public class MellatGateway
    {
        private string TerminalId;
        private string UserName;
        private string Password;
        PaymentGatewayClient client = new PaymentGatewayClient();


        public MellatGateway(string terminalId, string userName, string password)
        {
            TerminalId = terminalId;
            Password = password;
            UserName = userName;
        }

        public async Task<bpPayRequestResponse> bpPayRequest(int amount, int paymentNumber, string callbackUrl)
        {
            var terminalId = long.Parse(TerminalId);
            bpPayRequestResponse response = await client.bpPayRequestAsync(terminalId,
                                                           UserName,
                                                           Password,
                                                           paymentNumber,
                                                           amount,
                                                           GetDate(),
                                                           GetTime(),
                                                           paymentNumber.ToString(),
                                                           callbackUrl,
                                                           Int64.Parse("0")
                                                           );
            return response;
        }

        public async Task<bpInquiryRequestResponse> bpInquiryRequest(long SaleOrderId, long SaleReferenceId)
        {
            var terminalId = long.Parse(TerminalId);
            bpInquiryRequestResponse response = await client.bpInquiryRequestAsync(
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
            bpVerifyRequestResponse response = await client.bpVerifyRequestAsync(
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
            bpSettleRequestResponse response = await client.bpSettleRequestAsync(
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
            bpReversalRequestResponse response = await client.bpReversalRequestAsync(
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
            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') +
                   DateTime.Now.Day.ToString().PadLeft(2, '0');
        }
        private string GetTime()
        {
            return DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') +
                   DateTime.Now.Second.ToString().PadLeft(2, '0');
        }


    }


}
