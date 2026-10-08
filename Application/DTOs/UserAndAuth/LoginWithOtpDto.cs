namespace Application.DTOs.UserAndAuth
{
    public class LoginWithOtpDto
    {
        public string PhoneNumber { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}