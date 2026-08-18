using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class OtpService : IOtpService
    {
        private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(120);

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISmsService _smsService;
        public OtpService(IUnitOfWork unitOfWork, ISmsService smsService)
        {
            _unitOfWork = unitOfWork;
            _smsService = smsService;
        }
        public async Task IssueOtpAsync(string phoneNumber, OtpPurpose purpose)
        {
            var recent = await _unitOfWork.OtpVerifications.GetLatestAsync(phoneNumber);
            if (recent is not null && DateTime.UtcNow - recent.CreatedAt < ResendCooldown && recent.Purpose == purpose)
                throw new InvalidOperationException("Please wait before requesting another code.");

            // Cryptographically secure :)
            var code = RandomNumberGenerator.GetInt32(100000, 1_000_000).ToString();

            var otp = new OtpVerification
            {
                PhoneNumber = phoneNumber,
                Purpose = purpose,
                CodeHash = Hash(code),
                ExpiresAt = DateTime.UtcNow.Add(OtpLifetime),
                IsUsed = false
            };

            await _unitOfWork.OtpVerifications.AddAsync(otp);
            await _unitOfWork.SaveChangesAsync();

            await _smsService.SendAsync(phoneNumber, $"Your verification code is {code}. It expires in {(int)OtpLifetime.TotalMinutes} minutes.");
        }

        public async Task<bool> ConsumeOtpAsync(string phoneNumber, string code)
        {
            var otp = await _unitOfWork.OtpVerifications.GetLatestAsync(phoneNumber);
            if (otp is null || otp.IsUsed || otp.ExpiresAt < DateTime.UtcNow) return false;
            if (otp.CodeHash != Hash(code)) return false;

            otp.IsUsed = true;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static string Hash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
