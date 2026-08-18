using Domain.Enums;

namespace Application.Interfaces
{
    public interface IOtpService
    {
        Task IssueOtpAsync(string phoneNumber, OtpPurpose purpose);
        Task<bool> ConsumeOtpAsync(string phoneNumber, string code);
    }
}
