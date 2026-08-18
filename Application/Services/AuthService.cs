using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using System.ComponentModel;
using System.Security.Claims;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private const string DefaultProfileUrl = "https://s3.ir-thr-at1.arvanstorage.ir/shams1384/shams1384%2FDefault%20Images%2Fprofile.png";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;

        public AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, 
                           ITokenService tokenService,
                           IOtpService otpService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _otpService = otpService;
        }

        public async Task RequestRegistrationOtpAsync(RequestOtpDto dto)
        {
            var existing = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.PhoneNumber);
            if (existing is not null)
                throw new InvalidOperationException("This phone number is already registered.");

            await _otpService.IssueOtpAsync(dto.PhoneNumber);
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

            return BuildAuthResponse(user);
        }

        public async Task RequestLoginOtpAsync(RequestOtpDto dto)
        {
            var user = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.PhoneNumber);
            if (user is null)
                throw new InvalidOperationException("No account found for this phone number.");

            await _otpService.IssueOtpAsync(dto.PhoneNumber);
        }

        public async Task<AuthResponseDto> VerifyLoginWithOtpAsync(LoginWithOtpDto dto)
        {
            var user = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.PhoneNumber);
            if (user is null) 
                throw new InvalidOperationException("No account found for this phone number.");

            var valid = await _otpService.ConsumeOtpAsync(dto.PhoneNumber, dto.Code);
            if (!valid) 
                throw new InvalidOperationException("Invalid or expired verification code.");

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginWithPasswordAsync(LoginWithPasswordDto dto)
        {
            var user = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.PhoneNumber);
            if (user is null || user.PasswordHash is null)
                throw new InvalidOperationException("Informations are not valid you sure registerd before?!");

            var valid = _passwordHasher.Verify(user.PasswordHash, dto.Password);
            if (!valid)
                throw new InvalidOperationException("Invalid phone number or password.");

            return BuildAuthResponse(user);
        }

        public async Task RequestAddPasswordOtpAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
                throw new InvalidOperationException("User not found.");

            if (!string.IsNullOrEmpty(user.PasswordHash))
                throw new InvalidOperationException("You already have a password.");

            await _otpService.IssueOtpAsync(user.PhoneNumber);
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

        private AuthResponseDto BuildAuthResponse(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.MobilePhone, user.PhoneNumber),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var token = _tokenService.GenerateAccessToken(claims);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfileUrl = user.ProfileUrl,
                Role = user.Role
            };
        }
    }
}