using Domain.Enums;

namespace Application.DTOs.SupportChat;

public class UpdateSupportConversationStatusDto
{
    public SupportConversationStatus Status { get; set; }
}