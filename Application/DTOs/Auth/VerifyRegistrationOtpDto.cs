namespace Application.DTOs.Auth
{
    public class VerifyRegistrationOtpDto
    {
        public string PhoneNumber { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}