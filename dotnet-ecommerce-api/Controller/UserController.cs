using Application.DTOs.UserAndAuth;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.Interfaces.UserAndAuth;
using Domain.Exceptions;

namespace dotnet_ecommerce_api.Controller
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger <UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var user = await _userService.GetByIdAsync(GetUserId());
            if (user is null) throw new NotFoundException("User not found.");
            return Ok(user);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromForm] UserRequest request)
        {
            Stream? imageStream = null;

            try
            {
                if (request.File is not null)
                    imageStream = request.File.OpenReadStream();

                var dto = new UpdateUserDto
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Image = imageStream,
                    ImageName = request.File?.FileName,
                    ContentType = request.File?.ContentType
                };

                var updated = await _userService.UpdateProfileAsync(GetUserId(), dto);

                if (!updated)
                    throw new NotFoundException("User not found.");

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
            if (!deleted) throw new NotFoundException("User not found.");
            
            _logger.LogInformation(
                "User soft deleted. AdminUserId {AdminUserId}, TargetUserId {TargetUserId}",
                GetUserId(),
                id);
            
            return NoContent();
        }
        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllDeletedUsers()
        {
            var users = await _userService.GetAllDeletedUsersAsync();
            return Ok(users);
        }

        [HttpPost("{id:int}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreUser(int id)
        {
            var restored = await _userService.RestoreUserAsync(id);

            if (!restored)
                throw new NotFoundException("Deleted user not found.");

            _logger.LogInformation(
                "User restored. AdminUserId {AdminUserId}, TargetUserId {TargetUserId}",
                GetUserId(),
                id);

            return NoContent();
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllActiveUsers()
        {
            var users = await _userService.GetAllActiveUsersAsync();
            return Ok(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        private int GetUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(value, out var userId))
                throw new UnauthorizedException("Invalid user identity.");

            return userId;
        }
    }
}

