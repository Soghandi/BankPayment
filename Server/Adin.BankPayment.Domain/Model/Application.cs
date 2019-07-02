using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adin.BankPayment.Domain.Model
{
    [Table("Application")]
    public class Application : BaseEntity
    {
        [StringLength(32)] public string Title { get; set; }

        [StringLength(256)] public string Description { get; set; }

        [StringLength(128)] public string PublicKey { get; set; }

        [StringLength(128)] public string PrivateKey { get; set; }
    }
}