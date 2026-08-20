using Domain.Enums;

namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PasswordHash { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string ProfileUrl { get; set; } = null!;
        public Guid ImageFolderId { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
