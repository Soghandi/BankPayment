using Adin.BankPayment.Connector.Enum;

namespace Adin.BankPayment.Connector.Model
{
    public class OutputModel<T>
    {
        public ApiStatusCodeEnum Status { get; set; }

        public T Body { get; set; }
    }
}