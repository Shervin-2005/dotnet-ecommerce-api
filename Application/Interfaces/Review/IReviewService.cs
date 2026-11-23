using Application.DTOs.Common;
using Application.DTOs.Review;
using Domain.Enums;

namespace Application.Interfaces.Review
{
    public interface IReviewService
    {
        Task<PagedResult<ReviewDto>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<ReviewDto>> GetAllPendingAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<ReviewDto>> GetAllApprovedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<ReviewDto>> GetAllRejectedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId);
        Task<ReviewDto> CreateAsync(int productId, int userId, CreateReviewDto dto);
        Task<ReviewActionResult> UpdateAsync(int reviewId, int userId, UpdateReviewDto dto);
        Task<ReviewActionResult> DeleteAsync(int reviewId, int userId, bool isAdmin);
        Task<ReviewActionResult> ApproveAsync(int reviewId);
        Task<ReviewActionResult> RejectAsync(int reviewId);
    }
}