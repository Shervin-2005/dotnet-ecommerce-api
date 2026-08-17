using Domain.Entities;

namespace Application.Interfaces
{
    public interface IOtpVerificationRepository : IGenericRepository<OtpVerification>
    {
        Task<OtpVerification?> GetLatestAsync(string phoneNumber);
    }
}
