using System.Text;
using System.Text.Json;
using IdentityTestConsole.Dtos;
using Microsoft.Extensions.Options;

namespace IdentityTestConsole.Services;

public class AuthTestService : IAuthTestService
{
    private readonly HttpClient _httpClient;
    private readonly TestSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthTestService(IHttpClientFactory factory, IOptions<TestSettings> options)
    {
        _httpClient = factory.CreateClient();
        _settings = options.Value;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task RegisterAsync(string email, string password)
    {
        var payload = new { Email = email, Password = password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_settings.IdentityServiceBaseUrl}/auth/register", content);

        if (response.IsSuccessStatusCode)
            Console.WriteLine("Registered successfully.");
        else
            Console.WriteLine($"Registration skipped or failed: {response.StatusCode}");
    }

    public async Task<(bool Success, string AccessToken, string RefreshToken)> LoginAsync(string email, string password)
    {
        var payload = new { Email = email, Password = password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_settings.IdentityServiceBaseUrl}/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Login failed.");
            return (false, "", "");
        }

        var body = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TokenResponse>(body, _jsonOptions);
        Console.WriteLine("✅ Login successful.");
        return (true, result!.AccessToken, result.RefreshToken);
    }

    public async Task<(bool Success, string AccessToken)> RefreshTokenAsync(string refreshToken)
    {
        var payload = new { RefreshToken = refreshToken };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_settings.IdentityServiceBaseUrl}/auth/refresh", content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Refresh failed.");
            return (false, "");
        }

        var body = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TokenResponse>(body, _jsonOptions);
        Console.WriteLine("🔄 Token refreshed successfully.");
        return (true, result!.AccessToken);
    }
}