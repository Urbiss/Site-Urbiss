using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Urbiss.Domain.Models
{
    public class Voucher : BaseEntity
    {
        [Column(TypeName = "character varying(15)")]
        [Required]
        public string Code { get; set; }

        [Column]
        [Required]
        public double Area { get; set; }

        [Column]
        public DateTime Expiration { get; set; }

        [Column]
        [Required]
        public string Email { get; set; }

        [Column(TypeName = "character varying(10)")]
        [Required]
        public VoucherStatusEnum Status { get; set; } = VoucherStatusEnum.Pending;
    }

    public enum VoucherStatusEnum
    {
        Pending,
        Used,
        Canceled
    }
}
