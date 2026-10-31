using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.UserAndAuth
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task RevokeAllForUserAsync(int userId);
    }
}
