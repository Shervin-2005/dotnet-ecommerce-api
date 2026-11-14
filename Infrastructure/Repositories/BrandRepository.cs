using Application.DTOs.Common;
using Application.Interfaces.Brand;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BrandRepository : GenericRepository<Brand>, IBrandRepository
{
    private readonly AppDbContext _context;

    public BrandRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedResult<Brand>> GetAllPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Brands.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var brands = await query
            .OrderBy(b => b.BrandId)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Brand>
        {
            Items = brands,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}