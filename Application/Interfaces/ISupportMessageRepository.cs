using Domain.Entities;

namespace Application.Interfaces;

public interface ISupportMessageRepository : IGenericRepository<SupportMessage>
{
    Task<List<SupportMessage>> GetConversationMessagesAsync(int conversationId);
}