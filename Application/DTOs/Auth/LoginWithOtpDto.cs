namespace Application.DTOs.Auth
{
    public class LoginWithOtpDto
    {
        public string PhoneNumber { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}