using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Urbiss.Domain.Models
{
    public class Order : BaseEntity
    {
        [Column(TypeName = "character varying(40)")]
        [Required]
        public string OrderId { get; set; }

        [Column]
        [Required]
        public int UserId { get; set; }

        [Column]
        [Required]
        public DateTime Date { get; set; }

        [Column]
        [Required]
        public long UserSurveyId { get; set; }
        public UserSurvey UserSurvey { get; set; }

        [Column]
        [Required]
        public long SurveyId { get; set; }

        [Column]
        [Required]
        public double Price { get; set; }

        [Column]
        public long? VoucherId { get; set; }

        [Column(TypeName = "character varying(15)")]
        [Required]
        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Created;
    }

    public enum OrderStatusEnum
    {
        Created,
        Processed
    }
}
