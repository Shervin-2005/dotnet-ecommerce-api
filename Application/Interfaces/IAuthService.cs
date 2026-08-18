using Application.DTOs.Auth;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task RequestRegistrationOtpAsync(RequestOtpDto dto);
        Task<AuthResponseDto> VerifyRegistrationOtpAsync(VerifyRegistrationOtpDto dto);
        Task RequestLoginOtpAsync(RequestOtpDto dto);
        Task<AuthResponseDto?> VerifyLoginWithOtpAsync(LoginWithOtpDto dto);
        Task<AuthResponseDto?> LoginWithPasswordAsync(LoginWithPasswordDto dto);
        Task RequestAddPasswordOtpAsync(int userId);
        Task<AddPasswordResult> VerifyAddPasswordAsync(int userId, string otpCode, string newPassword);
        Task<ChangePasswordResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}