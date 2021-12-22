using System.ComponentModel.DataAnnotations;

namespace Urbiss.Domain.Dtos
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
