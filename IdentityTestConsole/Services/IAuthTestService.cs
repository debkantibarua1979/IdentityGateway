namespace IdentityTestConsole.Services;

public interface IAuthTestService
{
    Task RegisterAsync(string email, string password);
    Task<(bool Success, string AccessToken, string RefreshToken)> LoginAsync(string email, string password);
    Task<(bool Success, string AccessToken)> RefreshTokenAsync(string refreshToken);
}