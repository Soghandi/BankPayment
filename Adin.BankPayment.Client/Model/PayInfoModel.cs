using Adin.BankPayment.Client.Enum;
using System;

namespace Adin.BankPayment.Client
{
    public class PayInfoModel
    {
        public string PublicKey { get; set; }

        public string CallbackUrl { get; set; }

        public string TrackCode { get; set; }

        public long Amount { get; set; }

        public long? Mobile { get; set; }

        public string SecondTrackCode { get; set; } = "";

        public PriceUnitEnum PriceUnit { get; set; } = PriceUnitEnum.Default;

        public BankCodeEnum BankCode { get; set; } = BankCodeEnum.Default;
    }
}
