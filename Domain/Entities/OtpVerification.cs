namespace Domain.Entities
{
    public class OtpVerification
    {
        public int OtpVerificationId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string CodeHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
