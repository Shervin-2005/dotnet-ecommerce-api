using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class LoginWithOtpDto
    {
        [Required, Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = null!;
    }
}