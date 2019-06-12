using System;
using Adin.BankPayment.Connector.Enum;

namespace Adin.BankPayment.Connector.Model
{
    public class PayInfoModel
    {
        public string CallbackUrl { get; set; }

        public string TrackCode { get; set; }

        public decimal Amount { get; set; }

        public long? Mobile { get; set; }

        public string SecondTrackCode { get; set; } = "";

        public DateTime? ExpirationTime { get; set; }

        public PriceUnitEnum PriceUnit { get; set; } = PriceUnitEnum.Default;

        public BankCodeEnum BankCode { get; set; } = BankCodeEnum.Default;
    }
}