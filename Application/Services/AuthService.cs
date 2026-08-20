using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Settings;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private const string DefaultProfileUrl = "https://s3.ir-thr-at1.arvanstorage.ir/shams1384/shams1384%2FDefault%20Images%2Fprofile.png";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, 
                           ITokenService tokenService,
                           IOtpService otpService,
                           IOptions<JwtSettings> jwtSettings)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _otpService = otpService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task RequestRegistrationOtpAsync(RequestOtpDto dto)
        {
            var existing = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.PhoneNumber);
            if (existing is not null)
                throw new InvalidOperationException("This phone number is already registered.");

            await _otpService.IssueOtpAsync(dto.PhoneNumber, OtpPurpose.Registration);
        }

        public async Task<AuthResponseDto> VerifyRegistrationOtpAsync(VerifyRegistrationOtpDto dto)
        {
            var existing = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.PhoneNumber);
            if (existing is not null)
                throw new InvalidOperationException("This phone number is already registered.");

            var valid = await _otpService.ConsumeOtpAsync(dto.PhoneNumber, dto.Code);
            if (!valid)
                throw new InvalidOperationException("Invalid or expired verification code.");

            var user = new User
            {
                PhoneNumber = dto.PhoneNumber,
                Role = UserRole.Customer,
                ProfileUrl = DefaultProfileUrl,
            };

            var folderId = Guid.NewGuid();
            user.ImageFolderId = folderId;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return await IssueTokensAsync(user);

        }

        public async Task RequestLoginOtpAsync(RequestOtpDto dto)
        {
            var user = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.PhoneNumber);
            if (user is null)
                throw new InvalidOperationException("No account found for this phone number.");

            await _otpService.IssueOtpAsync(dto.PhoneNumber, OtpPurpose.Login);
        }

        public async Task<AuthResponseDto> VerifyLoginWithOtpAsync(LoginWithOtpDto dto)
        {
            var user = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.PhoneNumber);
            if (user is null) 
                throw new InvalidOperationException("No account found for this phone number.");

            var valid = await _otpService.ConsumeOtpAsync(dto.PhoneNumber, dto.Code);
            if (!valid) 
                throw new InvalidOperationException("Invalid or expired verification code.");

            return await IssueTokensAsync(user);
        }

        public async Task<AuthResponseDto> LoginWithPasswordAsync(LoginWithPasswordDto dto)
        {
            var user = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.PhoneNumber);
            if (user is null || user.PasswordHash is null)
                throw new InvalidOperationException("Informations are not valid you sure registerd before?!");

            var valid = _passwordHasher.Verify(user.PasswordHash, dto.Password);
            if (!valid)
                throw new InvalidOperationException("Invalid phone number or password.");

            return await IssueTokensAsync(user);
        }

        public async Task RequestPhoneChangeAsync(int userId, RequestPhoneChangeDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user is null)
                throw new InvalidOperationException("User not found.");

            if (user.PhoneNumber == dto.NewPhoneNumber)
                throw new InvalidOperationException(
                    "This is already your phone number.");

            var existing = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.NewPhoneNumber);

            if (existing is not null)
                throw new InvalidOperationException("This phone number is already registered.");

            await _otpService.IssueOtpAsync(dto.NewPhoneNumber, OtpPurpose.ChangePhoneNumber);
        }

        public async Task<bool> VerifyPhoneChangeAsync(int userId, VerifyPhoneChangeDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user is null) return false;

            var existing = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.NewPhoneNumber);

            if (existing is not null) throw new InvalidOperationException("This phone number is already registered.");

            var valid = await _otpService.ConsumeOtpAsync(dto.NewPhoneNumber, dto.Code);

            if (!valid) throw new InvalidOperationException("Invalid or expired verification code.");

            user.PhoneNumber = dto.NewPhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task RequestAddPasswordOtpAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
                throw new InvalidOperationException("User not found.");

            if (!string.IsNullOrEmpty(user.PasswordHash))
                throw new InvalidOperationException("You already have a password.");

            await _otpService.IssueOtpAsync(user.PhoneNumber, OtpPurpose.AddPassword);
        }

        public async Task<AddPasswordResult> VerifyAddPasswordAsync(int userId, string otpCode, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) return AddPasswordResult.UserNotFound;

            if (!string.IsNullOrEmpty(user.PasswordHash))
                return AddPasswordResult.PasswordAlreadyExists;

            var isOtpValid = await _otpService.ConsumeOtpAsync(user.PhoneNumber, otpCode);
            if (!isOtpValid) return AddPasswordResult.InvalidOtp;

            user.PasswordHash = _passwordHasher.Hash(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return AddPasswordResult.Success;
        }

        public async Task<ChangePasswordResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) return ChangePasswordResult.UserNotFound;

            if (string.IsNullOrEmpty(user.PasswordHash))
                return ChangePasswordResult.CurrentPasswordNotFound;

            var isCurrentPasswordValid = _passwordHasher.Verify(user.PasswordHash, currentPassword);
            if (!isCurrentPasswordValid) return ChangePasswordResult.IncorrectCurrentPassword;

            user.PasswordHash = _passwordHasher.Hash(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ChangePasswordResult.Success;
        }
        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var tokenHash = Hash(dto.RefreshToken);
            var stored = await _unitOfWork.RefreshTokens.GetByTokenHashAsync(tokenHash);

            if (stored is null || stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
                return null;

            var user = await _unitOfWork.Users.GetByIdAsync(stored.UserId);
            if (user is null) return null;

            stored.IsRevoked = true;
            stored.RevokedAt = DateTime.UtcNow;

            return await IssueTokensAsync(user); // saves the revocation and create the new token together
        }

        public async Task<AuthResponseDto> ReissueTokensAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            await _unitOfWork.RefreshTokens.RevokeAllForUserAsync(userId);

            return await IssueTokensAsync(user); // saves the revocations + the new token together
        }

        public async Task LogoutAsync(RefreshTokenDto dto)
        {
            var tokenHash = Hash(dto.RefreshToken);
            var stored = await _unitOfWork.RefreshTokens.GetByTokenHashAsync(tokenHash);

            if (stored is null || stored.IsRevoked) return; 

            stored.IsRevoked = true;
            stored.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task<AuthResponseDto> IssueTokensAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.MobilePhone, user.PhoneNumber),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = Hash(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.UserId,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };
        }
        private static string Hash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}