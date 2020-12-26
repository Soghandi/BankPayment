using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adin.BankPayment.Domain.Model
{
    [Table("Bank")]
    public class Bank : BaseEntity
    {
        [StringLength(64)]
        public string Title { get; set; }

        [StringLength(64)]
        public string PostUrl { get; set; }

        public byte Code { get; set; }
    }
}