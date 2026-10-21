using Application.DTOs.SupportChat;

namespace Application.Interfaces.SupportChat;

public interface ISupportChatNotifier
{
    Task NotifyMessageSentAsync(int conversationId, SupportMessageDto message);
}