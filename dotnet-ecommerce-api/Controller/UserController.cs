using Application.DTOs;
using Application.DTOs.Auth;
using Application.Interfaces;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace dotnet_ecommerce_api.Controller
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var user = await _userService.GetByIdAsync(GetUserId());
            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(UserRequest request)
        {
            Stream? imageStream = null;

            try
            {
                //later should alter this with fluent validation 
                if (request.File is not null && request.File.Length > 0)
                {
                    const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
                    if (request.File.Length > maxSizeBytes)
                        return BadRequest("File too large. Max size is 5 MB.");

                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
                    if (!allowedTypes.Contains(request.File.ContentType))
                        return BadRequest("Unsupported file type. Use JPEG, PNG, or WebP.");

                    imageStream = request.File.OpenReadStream();
                }

                var dto = new UpdateUserDto
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Image = imageStream,
                    ImageName = request.File?.FileName,
                    ContentType = request.File?.ContentType
                };

                var updated = await _userService.UpdateProfileAsync(GetUserId(), dto);
                if (!updated) return NotFound();
                return NoContent();
            }
            finally
            {
                if (imageStream is not null)
                    await imageStream.DisposeAsync();
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        private int GetUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(value, out var userId))
                throw new UnauthorizedAccessException("Invalid user identity.");

            return userId;
        }
    }
}

