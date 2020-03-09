using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adin.BankPayment.Domain.Model
{
    [Table("ApplicationBank")]
    public class ApplicationBank : BaseEntity
    {
        [ForeignKey("Application")]
        public Guid ApplicationId { get; set; }

        [ForeignKey("Bank")]
        public Guid BankId { get; set; }

        public virtual Bank Bank { get; set; }

        public virtual Application Application { get; set; }

        public virtual ICollection<ApplicationBankParam> ApplicationBankParams { get; set; }
    }
}