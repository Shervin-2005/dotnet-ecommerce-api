using Domain.Enums;

namespace Domain.Entities
{
    public class OtpVerification
    {
        public int OtpVerificationId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string CodeHash { get; set; } = null!;
        public OtpPurpose Purpose { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
