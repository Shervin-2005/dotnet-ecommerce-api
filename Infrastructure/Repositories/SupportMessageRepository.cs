using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SupportMessageRepository : GenericRepository<SupportMessage>, ISupportMessageRepository
{
    public SupportMessageRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<SupportMessage>> GetConversationMessagesAsync(int conversationId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(message => message.Sender)
            .Where(message =>
                message.SupportConversationId == conversationId)
            .OrderBy(message => message.SentAt)
            .ThenBy(message => message.SupportMessageId)
            .ToListAsync();
    }
}