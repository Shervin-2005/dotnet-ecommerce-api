using Domain.Entities;

namespace Application.Interfaces
{
    public interface IReviewRepository : IGenericRepository<ProductReview>
    {
        Task<ProductReview?> GetByProductAndUserAsync(int productId, int userId);
        
        Task<List<ProductReview>> GetByProductAsync(int productId);
        
        Task<Dictionary<int, (double AverageRating, int Count)>> GetRatingSummariesAsync(IEnumerable<int> productIds);
    }
}