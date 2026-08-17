using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OtpVerificationRepository : GenericRepository<OtpVerification>, IOtpVerificationRepository
    {
        public OtpVerificationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<OtpVerification?> GetLatestAsync(string phoneNumber) =>
            await _dbSet
                .Where(o => o.PhoneNumber == phoneNumber)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
    }
}
