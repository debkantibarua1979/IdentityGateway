using IdentityService.Dtos;

namespace IdentityService.Repositories.Interfaces;

public interface ITokenRepository
{
    Task SaveAccessTokenAsync(AccessToken token);
    Task SaveRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
    Task InvalidateRefreshTokenAsync(string refreshToken);
}