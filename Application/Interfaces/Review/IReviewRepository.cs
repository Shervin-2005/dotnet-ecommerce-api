using Application.DTOs.Common;
using Application.Interfaces.Common;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Review
{
    public interface IReviewRepository : IGenericRepository<ProductReview>
    {
        Task<PagedResult<ProductReview>> GetAllPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
      
        Task<PagedResult<ProductReview>> GetByStatusPagedAsync(ReviewStatus status, PaginationRequest request, CancellationToken cancellationToken = default);

        Task<ProductReview?> GetByProductAndUserAsync(int productId, int userId);
        
        Task<List<ProductReview>> GetByProductAsync(int productId);
        
        Task<Dictionary<int, (double AverageRating, int Count)>> GetRatingSummariesAsync(IEnumerable<int> productIds);

        Task<ProductReview?> GetByIdWithUserAsync(int reviewId);
    }
}