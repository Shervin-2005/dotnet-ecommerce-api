using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class LoginWithPasswordDto
    {
        [Required, Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}