using Domain.Entities;

namespace Application.Interfaces.SupportChat;

public interface ISupportMessageRepository : IGenericRepository<SupportMessage>
{
    Task<List<SupportMessage>> GetConversationMessagesAsync(int conversationId);
}