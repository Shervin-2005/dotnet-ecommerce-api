using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class RequestOtpDto
    {
        [Required, Phone]
        public string PhoneNumber { get; set; } = null!;
    }
}
