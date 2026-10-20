namespace Application.DTOs.SupportChat;

public class SupportMessageDto
{
    public int SupportMessageId { get; set; }

    public int SupportConversationId { get; set; }

    public int SenderId { get; set; }

    public string SenderName { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }
}