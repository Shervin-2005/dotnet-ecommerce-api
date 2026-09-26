namespace Application.DTOs.Auth
{
    public class VerifyPhoneChangeDto
    {
        public string NewPhoneNumber { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}