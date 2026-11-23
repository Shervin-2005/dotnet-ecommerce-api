using Application.DTOs.Common;
using Application.DTOs.Review;
using Application.Interfaces.Common;
using Application.Interfaces.Review;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<ReviewDto>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _unitOfWork.Reviews.GetAllPagedAsync(request, cancellationToken);

            return new PagedResult<ReviewDto>
            {
                Items = _mapper.Map<List<ReviewDto>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
        public Task<PagedResult<ReviewDto>> GetAllPendingAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            return GetByStatusAsync(ReviewStatus.Pending, request, cancellationToken);
        }

        public Task<PagedResult<ReviewDto>> GetAllApprovedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            return GetByStatusAsync(ReviewStatus.Approved, request, cancellationToken);
        }

        public Task<PagedResult<ReviewDto>> GetAllRejectedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            return GetByStatusAsync(ReviewStatus.Rejected, request, cancellationToken);
        }
        
        public async Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetByProductAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto> CreateAsync(int productId, int userId, CreateReviewDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product is null)
                throw new NotFoundException("Product not found.");

            var existing = await _unitOfWork.Reviews.GetByProductAndUserAsync(productId, userId);
            if (existing is not null)
                throw new ConflictException("You have already reviewed this product.");

            var review = new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
            
            review.User = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new NotFoundException("User not found.");

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewActionResult> UpdateAsync(int reviewId, int userId, UpdateReviewDto dto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review is null) return ReviewActionResult.NotFound;
            if (review.UserId != userId) return ReviewActionResult.Forbidden;

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();
            return ReviewActionResult.Success;
        }

        public async Task<ReviewActionResult> DeleteAsync(int reviewId, int userId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);

            if (review is null)
                return ReviewActionResult.NotFound;

            if (review.UserId != userId)
                return ReviewActionResult.Forbidden;

            return await DeleteReviewAsync(review);
        }

        public async Task<ReviewActionResult> AdminDeleteAsync(int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);

            if (review is null)
                return ReviewActionResult.NotFound;

            return await DeleteReviewAsync(review);
        }

        private async Task<ReviewActionResult> DeleteReviewAsync(ProductReview review)
        {
            _unitOfWork.Reviews.Delete(review);

            await _unitOfWork.SaveChangesAsync();

            return ReviewActionResult.Success;
        }
        
        public async Task<ReviewActionResult> ApproveAsync(int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);

            if (review == null)
                return ReviewActionResult.NotFound;

            if (review.Status != ReviewStatus.Pending)
                return ReviewActionResult.Forbidden;

            review.Status = ReviewStatus.Approved;
            review.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Reviews.Update(review);

            await _unitOfWork.SaveChangesAsync();

            return ReviewActionResult.Success;
        }
        
        public async Task<ReviewActionResult> RejectAsync(int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);

            if (review == null)
                return ReviewActionResult.NotFound;

            if (review.Status != ReviewStatus.Pending)
                return ReviewActionResult.Forbidden;

            review.Status = ReviewStatus.Rejected;
            review.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Reviews.Update(review);

            await _unitOfWork.SaveChangesAsync();

            return ReviewActionResult.Success;
        }
        
        private async Task<PagedResult<ReviewDto>> GetByStatusAsync(ReviewStatus status, PaginationRequest request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Reviews.GetByStatusPagedAsync(status, request, cancellationToken);

            return new PagedResult<ReviewDto>
            {
                Items = _mapper.Map<List<ReviewDto>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}