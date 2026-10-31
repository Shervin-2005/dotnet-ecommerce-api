using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.SupportChat
{
    public interface ISupportConversationRepository : IGenericRepository<SupportConversation>
    {
        Task<SupportConversation?> GetActiveByUserIdAsync(int userId);

        Task<SupportConversation?> GetWithUsersAsync(int conversationId);

        Task<List<SupportConversation>> GetAllWithUsersAsync();
        
        Task<int> DeleteExpiredClosedAsync(DateTime cutoffDate);
    }
}