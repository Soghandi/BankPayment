using Adin.BankPayment.Connector.Model;
using Adin.BankPayment.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Adin.BankPayment.Extension
{
    public interface BankHelper
    {
        Task<VerifyTransactionResponseModel> VerifyTransaction(Transaction transaction);
    }

    
}
