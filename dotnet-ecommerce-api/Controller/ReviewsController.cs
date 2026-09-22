using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/products/{productId:int}/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByProduct(int productId)
        {
            var reviews = await _reviewService.GetByProductAsync(productId);
            return Ok(reviews);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewDto>> Create(int productId, CreateReviewDto dto)
        {
            var review = await _reviewService.CreateAsync(productId, GetUserId(), dto); 
            return CreatedAtAction(nameof(GetByProduct), new { productId }, review);
        }

        [Authorize]
        [HttpPut("{reviewId:int}")]
        public async Task<IActionResult> Update(int productId, int reviewId, UpdateReviewDto dto)
        {
            var result = await _reviewService.UpdateAsync(reviewId, GetUserId(), dto);
            return result switch
            {
                ReviewActionResult.Success => NoContent(),
                ReviewActionResult.NotFound => NotFound(),
                ReviewActionResult.Forbidden => Forbid(),
                _ => BadRequest()
            };
        }

        [Authorize]
        [HttpDelete("{reviewId:int}")]
        public async Task<IActionResult> Delete(int productId, int reviewId)
        {
            var isAdmin = User.IsInRole("Admin");
            var result = await _reviewService.DeleteAsync(reviewId, GetUserId(), isAdmin);
            return result switch
            {
                ReviewActionResult.Success => NoContent(),
                ReviewActionResult.NotFound => NotFound(),
                ReviewActionResult.Forbidden => Forbid(),
                _ => BadRequest()
            };
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}