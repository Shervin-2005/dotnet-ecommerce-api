using Application.DTOs.Common;
using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.SupportChat
{
    public interface ISupportConversationRepository : IGenericRepository<SupportConversation>
    {
        Task<SupportConversation?> GetActiveByUserIdAsync(int userId);
        Task<SupportConversation?> GetWithUsersAsync(int conversationId);
        Task<PagedResult<SupportConversation>> GetAllWithUsersAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<int> DeleteExpiredClosedAsync(DateTime cutoffDate);
    }
}