using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Urbiss.Domain.Models
{
    public class User : IdentityUser<int>
    {
        [Column(TypeName = "character varying(150)")]
        [Required]
        public string FullName { get; set; }

        [Column]
        [Required]
        public bool IsActive { get; set; } = false;

        public List<UserRole> UserRoles { get; set; }
    }
}
