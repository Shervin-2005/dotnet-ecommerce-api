using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class VerifyPhoneChangeDto
    {
        [Required, Phone]
        public string NewPhoneNumber { get; set; } = null!;

        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = null!;
    }
}
