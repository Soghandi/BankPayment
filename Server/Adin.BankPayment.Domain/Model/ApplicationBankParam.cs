using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adin.BankPayment.Domain.Model
{
    [Table("ApplicationBankParam")]
    public class ApplicationBankParam : BaseEntity
    {
        [ForeignKey("ApplicationBank")] public Guid ApplicationBankId { get; set; }

        [StringLength(32)] public string ParamKey { get; set; }

        [StringLength(128)] public string ParamValue { get; set; }

        public virtual ApplicationBank ApplicationBank { get; set; }
    }
}