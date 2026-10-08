namespace Application.DTOs.UserAndAuth
{
    public class LoginWithPasswordDto
    {
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}