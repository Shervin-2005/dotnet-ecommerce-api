using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash) =>
            await _dbSet.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

    }
}
