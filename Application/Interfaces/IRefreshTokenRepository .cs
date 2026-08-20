using Domain.Entities;

namespace Application.Interfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task RevokeAllForUserAsync(int userId);
    }
}
