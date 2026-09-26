namespace Application.DTOs.Auth
{
    public class LoginWithPasswordDto
    {
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}