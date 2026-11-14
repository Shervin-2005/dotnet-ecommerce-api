using Application.DTOs.Common;
using Application.Interfaces.Category;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
        
        public async Task<PagedResult<Category>> GetAllPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.Categories.AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var categories = await query
                .OrderBy(c => c.CategoryId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Category>
            {
                Items = categories,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
