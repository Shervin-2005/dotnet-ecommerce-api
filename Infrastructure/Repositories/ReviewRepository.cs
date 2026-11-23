using Application.DTOs.Common;
using Application.Interfaces.Review;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<ProductReview>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }
        
        public async Task<PagedResult<ProductReview>> GetAllPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var totalCount = await _dbSet.CountAsync(cancellationToken);

            var pageIds = await _dbSet
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ThenByDescending(r => r.ProductReviewId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => r.ProductReviewId)
                .ToListAsync(cancellationToken);

            var reviews = await _dbSet
                .AsNoTracking()
                .Where(r => pageIds.Contains(r.ProductReviewId))
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .ThenByDescending(r => r.ProductReviewId)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductReview>
            {
                Items = reviews,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }     
        
        public async Task<PagedResult<ProductReview>> GetByStatusPagedAsync(ReviewStatus status, PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(r => r.Status == status);

            var totalCount = await query.CountAsync(cancellationToken);

            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .ThenByDescending(r => r.ProductReviewId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(r => r.User)
                .Include(r => r.Product)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductReview>
            {
                Items = reviews,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        
        public async Task<ProductReview?> GetByProductAndUserAsync(int productId, int userId) =>
            await _dbSet.FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

        public async Task<List<ProductReview>> GetByProductAsync(int productId) =>
            await _dbSet
                .Where(r =>
                    r.ProductId == productId &&
                    r.Status == ReviewStatus.Approved)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<Dictionary<int, (double AverageRating, int Count)>> GetRatingSummariesAsync(IEnumerable<int> productIds)
        {
            var idList = productIds.ToList();

            var summaries = await _dbSet
                .Where(r =>
                    idList.Contains(r.ProductId) &&
                    r.Status == ReviewStatus.Approved)
                .GroupBy(r => r.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Average = g.Average(r => r.Rating),
                    Count = g.Count()
                })
                .ToListAsync();
            
            return summaries.ToDictionary(s => s.ProductId, s => (s.Average, s.Count));
        }

        public async Task<ProductReview?> GetByIdWithUserAsync(int reviewId) =>
            await _dbSet
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ProductReviewId == reviewId);
    }
}