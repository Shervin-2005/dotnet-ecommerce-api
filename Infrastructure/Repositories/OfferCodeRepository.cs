using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OfferCodeRepository
    : GenericRepository<OfferCode>, IOfferCodeRepository
{
    public OfferCodeRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<OfferCode?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<int> GetUserUsageCountAsync(
        int offerCodeId,
        int userId)
    {
        return await _dbSet
            .Where(x => x.OfferCodeId == offerCodeId)
            .SelectMany(x => x.Usages)
            .CountAsync(x => x.UserId == userId);
    }

    public async Task<List<OfferCode>> GetAllAsync()
    {
        return await _dbSet
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}