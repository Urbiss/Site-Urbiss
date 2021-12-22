using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Urbiss.Domain.Models
{
    public class BaseEntity
    {
        [Column("id")]
        [Required]
        public long Id { get; set; }

        [Column("usercreation")]
        [Required]        
        public string UserCreation { get; set; }

        [Column("creation")]
        [Required]
        public DateTime Creation { get; set; }

        [Column("usermodification")]
        public string UserModification { get; set; }

        [Column("modification")]
        public DateTime? Modification { get; set; }

    }
}
