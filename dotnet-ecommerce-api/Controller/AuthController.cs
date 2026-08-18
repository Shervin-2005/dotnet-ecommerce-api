using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Services;
using Domain.Enums;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace dotnet_ecommerce_api.Controller
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        // remove try catch after create global exception hanlder middleware
        [HttpPost("register/request-otp")]
        public async Task<IActionResult> RequsetRegistrationOtp(RequestOtpDto dto)
        {
            try
            {
                await _authService.RequestRegistrationOtpAsync(dto);
                return Ok(new { message = "Verification code sent." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("register/verify")]
        public async Task<ActionResult<AuthResponseDto>> VerifyRegistration(VerifyRegistrationOtpDto dto)
        {
            try
            {
                var response = await _authService.VerifyRegistrationOtpAsync(dto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login/request-otp")]
        public async Task<IActionResult> RequestLoginOtp(RequestOtpDto dto)
        {
            try
            {
                await _authService.RequestLoginOtpAsync(dto);
                return Ok(new { message = "Verification code sent." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login/otp")]
        public async Task<ActionResult<AuthResponseDto>> LoginWithOtp(LoginWithOtpDto dto)
        {
            var response = await _authService.VerifyLoginWithOtpAsync(dto);
            if (response is null) return Unauthorized("Invalid or expired code.");
            return Ok(response);
        }

        [HttpPost("login/password")]
        public async Task<ActionResult<AuthResponseDto>> LoginWithPassword(LoginWithPasswordDto dto)
        {
            var response = await _authService.LoginWithPasswordAsync(dto);
            if (response is null) return Unauthorized("Invalid phone number or password.");
            return Ok(response);
        }

        [Authorize]
        [HttpPost("me/password/otp")]
        public async Task<IActionResult> RequestAddPasswordOtp()
        {
            try
            {
                await _authService.RequestAddPasswordOtpAsync(GetUserId());
                return Ok(new { message = "Verification code sent." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("me/password")]
        public async Task<IActionResult> AddPassword(AddPasswordDto dto)
        {
            //later should alter this with fluent validation 
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || string.IsNullOrWhiteSpace(dto.Otp))
                return BadRequest("New password and OTP are required.");

            if (dto.NewPassword != dto.ConfirmNewPassword)
                return BadRequest("New password and confirmation do not match.");

            if (dto.NewPassword.Length < 8)
                return BadRequest("New password must be at least 8 characters long.");

            var result = await _authService.VerifyAddPasswordAsync(GetUserId(), dto.Otp, dto.NewPassword);

            return result switch
            {
                AddPasswordResult.Success => NoContent(),
                AddPasswordResult.UserNotFound => NotFound(),
                AddPasswordResult.PasswordAlreadyExists => Conflict("You already have a password. Use change password instead."),
                AddPasswordResult.InvalidOtp => BadRequest("Invalid or expired OTP."),
                _ => BadRequest("Unable to add password.")
            };
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            //later should alter this with fluent validation 
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest("Current and new password are required.");

            if (request.NewPassword != request.ConfirmNewPassword)
                return BadRequest("New password and confirmation do not match.");

            if (request.NewPassword.Length < 8)
                return BadRequest("New password must be at least 8 characters long.");

            if (request.CurrentPassword == request.NewPassword)
                return BadRequest("New password must be different from the current password.");

            var result = await _authService.ChangePasswordAsync(GetUserId(), request.CurrentPassword, request.NewPassword);

            //used an enum instead of bool because "user not found" (404) and "wrong current password" (400) need different HTTP responses
            return result switch
            {
                ChangePasswordResult.Success => NoContent(),
                ChangePasswordResult.UserNotFound => NotFound(),
                ChangePasswordResult.CurrentPasswordNotFound => BadRequest("You don't have any password yet"),
                ChangePasswordResult.IncorrectCurrentPassword => BadRequest("Current password is incorrect."),
                _ => BadRequest("Unable to change password.")
            };
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
