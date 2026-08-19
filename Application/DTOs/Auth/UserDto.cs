
using Domain.Enums;

namespace Application.DTOs.Auth
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string ProfileUrl { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public UserRole Role { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
