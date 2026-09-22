using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Settings;
using Domain.Enums;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace dotnet_ecommerce_api.Controller
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IAuthService authService, IOptions<JwtSettings> jwtSettings)
        {
            _authService = authService;
            _jwtSettings = jwtSettings.Value;
        }
        
        [HttpPost("register/request-otp")]
        public async Task<IActionResult> RequsetRegistrationOtp(RequestOtpDto dto)
        { 
            await _authService.RequestRegistrationOtpAsync(dto); 
            return Ok(new { message = "Verification code sent." });
        }
        [HttpPost("register/verify")]
        public async Task<ActionResult<AuthResponseDto>> VerifyRegistration(VerifyRegistrationOtpDto dto)
        {
            var response = await _authService.VerifyRegistrationOtpAsync(dto);

            SetAuthCookies(response);

            return Ok();
        }

        [HttpPost("login/request-otp")]
        public async Task<IActionResult> RequestLoginOtp(RequestOtpDto dto)
        {
            await _authService.RequestLoginOtpAsync(dto);
            return Ok(new { message = "Verification code sent." });
        }

        [HttpPost("login/otp")]
        public async Task<ActionResult<AuthResponseDto>> LoginWithOtp(LoginWithOtpDto dto)
        {
            var response = await _authService.VerifyLoginWithOtpAsync(dto);
            if (response is null) return Unauthorized("Invalid or expired code.");

            SetAuthCookies(response);

            return Ok();
        }

        [HttpPost("login/password")]
        public async Task<ActionResult<AuthResponseDto>> LoginWithPassword(LoginWithPasswordDto dto)
        {
            var response = await _authService.LoginWithPasswordAsync(dto);
            if (response is null) return Unauthorized("Invalid phone number or password.");

            SetAuthCookies(response);

            return Ok();
        }

        [Authorize]
        [HttpPost("me/phone/request")]
        public async Task<IActionResult> RequestPhoneChange(RequestPhoneChangeDto dto)
        {
            await _authService.RequestPhoneChangeAsync(GetUserId(), dto);
            
            return Ok(new
            {
                    message = "Verification code sent to the new phone number."
            });
        }

        [HttpPost("me/phone/verify")]
        public async Task<ActionResult<AuthResponseDto>> VerifyPhoneChange(VerifyPhoneChangeDto dto)
        {
            var userId = GetUserId(); 
            
            await _authService.VerifyPhoneChangeAsync(userId, dto);

            //new tokens for new phone number
            var response = await _authService.ReissueTokensAsync(userId);

            SetAuthCookies(response);

            return Ok();
        }

        [Authorize]
        [HttpPost("me/password/otp")]
        public async Task<IActionResult> RequestAddPasswordOtp()
        {

            await _authService.RequestAddPasswordOtpAsync(GetUserId());
            return Ok(new { message = "Verification code sent." });
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
            
            return result switch
            {
                ChangePasswordResult.Success => NoContent(),
                ChangePasswordResult.UserNotFound => NotFound(),
                ChangePasswordResult.CurrentPasswordNotFound => BadRequest("You don't have any password yet"),
                ChangePasswordResult.IncorrectCurrentPassword => BadRequest("Current password is incorrect."),
                _ => BadRequest("Unable to change password.")
            };
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            // Read refresh token from HttpOnly cookie
            if (!Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            {
                return Unauthorized("Refresh token is missing.");
            }

            var dto = new RefreshTokenDto
            {
                RefreshToken = refreshToken
            };

            var response = await _authService.RefreshTokenAsync(dto);

            if (response is null)
            {
                // Remove invalid cookies
                DeleteAuthCookies();

                return Unauthorized("Invalid or expired refresh token.");
            }

            SetAuthCookies(response);

            return Ok();
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            {
                var dto = new RefreshTokenDto
                {
                    RefreshToken = refreshToken
                };

                await _authService.LogoutAsync(dto);
            }

            // Remove tokens from browser
            DeleteAuthCookies();

            return NoContent();
        }

        private void SetAuthCookies(AuthResponseDto response)
        {
            var accessTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes)
            };

            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
            };

            Response.Cookies.Append("AccessToken", response.AccessToken, accessTokenOptions);

            Response.Cookies.Append("RefreshToken", response.RefreshToken, refreshTokenOptions);
        }


        private void DeleteAuthCookies()
        {
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");
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
