using Domain.Enums;

namespace Application.DTOs.SupportChat;

public class SupportConversationDto
{
    public int SupportConversationId { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public int? AssignedAdminId { get; set; }

    public string? AssignedAdminName { get; set; }

    public SupportConversationStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }
}