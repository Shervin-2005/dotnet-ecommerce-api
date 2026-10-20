using Application.DTOs.SupportChat;

namespace Application.Interfaces;

public interface ISupportChatNotifier
{
    Task NotifyMessageSentAsync(int conversationId, SupportMessageDto message);
}