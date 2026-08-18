using Domain.Enums;

namespace Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string ProfileUrl { get; set; } = null!;
        public UserRole Role { get; set; }
    }
}
