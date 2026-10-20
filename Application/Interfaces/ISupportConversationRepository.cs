using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISupportConversationRepository : IGenericRepository<SupportConversation>
    {
        Task<SupportConversation?> GetActiveByUserIdAsync(int userId);

        Task<SupportConversation?> GetWithUsersAsync(int conversationId);

        Task<List<SupportConversation>> GetAllWithUsersAsync();
    }
}