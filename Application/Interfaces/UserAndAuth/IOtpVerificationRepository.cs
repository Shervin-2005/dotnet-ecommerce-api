using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.UserAndAuth
{
    public interface IOtpVerificationRepository : IGenericRepository<OtpVerification>
    {
        Task<OtpVerification?> GetLatestAsync(string phoneNumber);
        Task<List<OtpVerification>> GetOlderThanAsync(DateTime date);
    }
}
