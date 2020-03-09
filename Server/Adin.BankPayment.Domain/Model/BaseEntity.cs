using System;
using System.ComponentModel.DataAnnotations;

namespace Adin.BankPayment.Domain.Model
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public byte Status { get; set; }

        public DateTime CreationDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        [Timestamp] 
        public byte[] RowVersion { get; set; }

        public bool IsDeleted { get; set; }
    }
}