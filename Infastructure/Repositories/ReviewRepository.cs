using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<ProductReview>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ProductReview?> GetByProductAndUserAsync(int productId, int userId) =>
            await _dbSet.FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

        public async Task<List<ProductReview>> GetByProductAsync(int productId) =>
            await _dbSet
                .Where(r => r.ProductId == productId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<Dictionary<int, (double AverageRating, int Count)>> GetRatingSummariesAsync(IEnumerable<int> productIds)
        {
            var idList = productIds.ToList();

            var summaries = await _dbSet
                .Where(r => idList.Contains(r.ProductId))
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
    }
}