
namespace Application.DTOs.Auth
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool HasPassword { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
