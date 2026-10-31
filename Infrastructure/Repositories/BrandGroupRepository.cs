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

    public async Task<IReadOnlyList<BrandGroup>> GetAllWithBrandsAsync()
    {
        return await _context.Set<BrandGroup>()
            .AsNoTracking()
            .Include(bg => bg.Brands)
            .OrderBy(bg => bg.DisplayOrder)
            .ToListAsync();
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