using Domain.Enums;

namespace Domain.Entities;

public class SupportConversation
{
    public int SupportConversationId { get; set; }

    public int UserId { get; set; }

    public int? AssignedAdminId { get; set; }

    public SupportConversationStatus Status { get; set; } = SupportConversationStatus.Open;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ClosedAt { get; set; }
    
    public User User { get; set; } = null!;

    public User? AssignedAdmin { get; set; }

    public ICollection<SupportMessage> Messages { get; set; } = new List<SupportMessage>();
}