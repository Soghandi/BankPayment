using System;
using System.Collections.Generic;
using System.Text;

namespace Adin.BankPayment.Connector.Model
{
    public class TokenModel
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }
    }
}
