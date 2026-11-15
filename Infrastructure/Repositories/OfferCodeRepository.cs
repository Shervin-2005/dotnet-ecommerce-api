using Application.DTOs.Common;
using Application.Interfaces.OfferCode;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OfferCodeRepository : GenericRepository<OfferCode>, IOfferCodeRepository
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

    public async Task<PagedResult<OfferCode>> GetAllPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var offerCodes = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.OfferCodeId)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<OfferCode>
        {
            Items = offerCodes,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}