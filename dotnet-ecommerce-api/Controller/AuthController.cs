using Application.DTOs.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }
}
