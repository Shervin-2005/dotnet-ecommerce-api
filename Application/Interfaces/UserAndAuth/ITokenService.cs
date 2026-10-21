using System.Security.Claims;

namespace Application.Interfaces.UserAndAuth
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
    }
}
