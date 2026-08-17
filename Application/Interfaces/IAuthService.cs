using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task RequestRegistrationOtpAsync(RequestOtpDto dto);
        Task<AuthResponseDto> VerifyRegistrationOtpAsync(VerifyRegistrationOtpDto dto);
        Task RequestLoginOtpAsync(RequestOtpDto dto);
        Task<AuthResponseDto?> VerifyLoginWithOtpAsync(LoginWithOtpDto dto);
        Task<AuthResponseDto?> LoginWithPasswordAsync(LoginWithPasswordDto dto);
    }
}