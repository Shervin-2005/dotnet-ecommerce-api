using Application.DTOs.Review;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId);
        Task<ReviewDto> CreateAsync(int productId, int userId, CreateReviewDto dto);
        Task<ReviewActionResult> UpdateAsync(int reviewId, int userId, UpdateReviewDto dto);
        Task<ReviewActionResult> DeleteAsync(int reviewId, int userId, bool isAdmin);
    }
}