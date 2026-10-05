using Application.DTOs.Review;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admin;

[ApiController]
[Route("api/admin/reviews")]
[Authorize(Roles = "Admin")]
public class AdminReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public AdminReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetPending()
    {
        var reviews = await _reviewService.GetPendingAsync();

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