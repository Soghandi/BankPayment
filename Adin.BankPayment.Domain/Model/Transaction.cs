using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Adin.BankPayment.Domain.Model
{
    [Table("Transaction")]
    public class Transaction : BaseEntity
    {

        [ForeignKey("Application")]
        public Guid ApplicationId { get; set; }

        [ForeignKey("Bank")]
        public Guid BankId { get; set; }

        public decimal Amount { get; set; }

        public long? Mobile { get; set; }

        [StringLength(64)]
        public string UserTrackCode { get; set; }

        [StringLength(64)]
        public string BankTrackCode { get; set; }

        [StringLength(64)]
        public string ReferenceNumber { get; set; }

        public int BankErrorCode { get; set; }

        [StringLength(128)]
        public string BankRedirectUrl { get; set; }

        [StringLength(128)]
        public string CallbackUrl { get; set; }

        public DateTime? ExpirationTime { get; set; }

        [StringLength(64)]
        public string BankErrorMessage { get; set; }

        public byte ErrorCode { get; set; }

        public virtual Application Application { get; set; }

        public virtual Bank Bank { get; set; }

        
    }
}
