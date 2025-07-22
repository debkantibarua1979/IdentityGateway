namespace IdentityService.Services.Interfaces;
using IdentityService.Dtos;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RefreshAccessTokenAsync(RefreshTokenRequest request);
    Task<GenericResult> RegisterAsync(RegisterRequest request);
}
