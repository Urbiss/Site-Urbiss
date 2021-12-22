using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Urbiss.Domain.Models
{
    public class Role : IdentityRole<int>
    {
        public Role(string roleName) : base(roleName)
        {

        }

        public Role() : base() { }

        public List<UserRole> UserRoles { get; set; }
    }
}
