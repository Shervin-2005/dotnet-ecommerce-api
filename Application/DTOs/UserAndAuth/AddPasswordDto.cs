namespace Application.DTOs.UserAndAuth
{
    public class AddPasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}
