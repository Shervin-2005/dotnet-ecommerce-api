namespace Domain.Entities;

public class SupportMessage
{
    public int SupportMessageId { get; set; }

    public int SupportConversationId { get; set; }

    public int SenderId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; }
    
    public SupportConversation Conversation { get; set; } = null!;

    public User Sender { get; set; } = null!;
}