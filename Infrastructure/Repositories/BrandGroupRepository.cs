using Application.DTOs.Common;
using Application.Interfaces.Brand;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BrandGroupRepository : GenericRepository<BrandGroup>, IBrandGroupRepository
{
    public BrandGroupRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<BrandGroup>> GetAllWithBrandsAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<BrandGroup>().AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var groups = await query
            .Include(bg => bg.Brands)
            .AsSplitQuery()
            .OrderBy(bg => bg.DisplayOrder)
            .ThenBy(bg => bg.BrandGroupId)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<BrandGroup>
        {
            Items = groups,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    
    public async Task<BrandGroup?> GetByIdWithBrandsAsync(int id)
    {
        return await _context.Set<BrandGroup>()
            .AsNoTracking()
            .Include(bg => bg.Brands)
            .FirstOrDefaultAsync(bg => bg.BrandGroupId == id);
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
    {
        var query = _context.Set<BrandGroup>()
            .Where(bg => bg.Slug == slug);

        if (excludeId.HasValue)
            query = query.Where(bg => bg.BrandGroupId != excludeId.Value);

        return await query.AnyAsync();
    }
}