using Domain.Entities;

namespace Application.Interfaces
{
    public interface IReviewRepository : IGenericRepository<ProductReview>
    {
        Task<ProductReview?> GetByProductAndUserAsync(int productId, int userId);
        
        Task<List<ProductReview>> GetByProductAsync(int productId);
        
        Task<Dictionary<int, (double AverageRating, int Count)>> GetRatingSummariesAsync(IEnumerable<int> productIds);
        
        Task<List<ProductReview>> GetPendingAsync();

        Task<ProductReview?> GetByIdWithUserAsync(int reviewId);
    }
}