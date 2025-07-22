namespace IdentityService.Repositories.Interfaces;

using IdentityService.Dtos;

public interface ITokenRepository
{
    Task SaveAccessTokenAsync(AccessToken token);
    Task<AccessToken?> GetAccessTokenAsync(string token);
    Task SaveRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
    Task InvalidateRefreshTokenAsync(string refreshToken);
}