using Application.DTOs.Common;
using Application.DTOs.Review;
using Application.Interfaces.Review;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller;

[Authorize(Roles = "Admin")]
public class AdminReviewsController : BaseController
{
    private readonly IReviewService _reviewService;

    
    public AdminReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetAll([FromQuery] PaginationRequest request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetAllAsync(request, cancellationToken);

        return Ok(reviews);
    }
    
    [HttpGet("approved")]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetAllApproved([FromQuery] PaginationRequest request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetAllApprovedAsync(request, cancellationToken);

        return Ok(reviews);
    }
    
    [HttpGet("pending")]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetAllPending([FromQuery] PaginationRequest request,CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetAllPendingAsync(
            request,
            cancellationToken);

        return Ok(reviews);
    }
    
    [HttpGet("rejected")]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetAllRejected([FromQuery] PaginationRequest request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetAllRejectedAsync(request, cancellationToken);

        return Ok(reviews);
    }

    [HttpPut("{reviewId:int}/approve")]
    public async Task<IActionResult> Approve(int reviewId)
    {
        var result = await _reviewService.ApproveAsync(reviewId);

        return result switch
        {
            ReviewActionResult.Success => NoContent(),
            ReviewActionResult.NotFound => NotFound(),
            ReviewActionResult.Forbidden => Conflict(),
            _ => BadRequest()
        };
    }

    [HttpPut("{reviewId:int}/reject")]
    public async Task<IActionResult> Reject(int reviewId)
    {
        var result = await _reviewService.RejectAsync(reviewId);

        return result switch
        {
            ReviewActionResult.Success => NoContent(),
            ReviewActionResult.NotFound => NotFound(),
            ReviewActionResult.Forbidden => Conflict(),
            _ => BadRequest()
        };
    }
}