using IdentityService.Dtos;

namespace IdentityService.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RefreshAccessTokenAsync(RefreshRequest request);
}