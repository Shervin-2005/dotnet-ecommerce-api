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

        public async Task<List<SupportConversation>> GetAllWithUsersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(conversation => conversation.User)
                .Include(conversation => conversation.AssignedAdmin)
                .OrderByDescending(conversation => conversation.CreatedAt)
                .ToListAsync();
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