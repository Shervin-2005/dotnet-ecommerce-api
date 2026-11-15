using Application.DTOs.Common;
using Application.Interfaces.SupportChat;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SupportConversationRepository : GenericRepository<SupportConversation>, ISupportConversationRepository
    {
        public SupportConversationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<SupportConversation?> GetActiveByUserIdAsync(int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(conversation =>
                conversation.UserId == userId &&
                (conversation.Status == SupportConversationStatus.Open ||
                 conversation.Status == SupportConversationStatus.InProgress));
        }

        public async Task<SupportConversation?> GetWithUsersAsync(int conversationId)
        {
            return await _dbSet
                .Include(conversation => conversation.User)
                .Include(conversation => conversation.AssignedAdmin)
                .FirstOrDefaultAsync(conversation =>
                    conversation.SupportConversationId == conversationId);
        }

        public async Task<PagedResult<SupportConversation>> GetAllWithUsersAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var conversations = await query
                .Include(c => c.User)
                .Include(c => c.AssignedAdmin)
                .OrderByDescending(c => c.CreatedAt)
                .ThenByDescending(c => c.SupportConversationId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<SupportConversation>
            {
                Items = conversations,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        
        public Task<int> DeleteExpiredClosedAsync(DateTime cutoffDate)
        {
            return _context.SupportConversations
                .Where(conversation =>
                    conversation.Status == SupportConversationStatus.Closed &&
                    conversation.ClosedAt != null &&
                    conversation.ClosedAt <= cutoffDate)
                .ExecuteDeleteAsync();
        }
    }
}