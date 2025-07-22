using System.Net.Http.Headers;
using System.Text.Json;
using IdentityTestConsole.Dtos;
using Microsoft.Extensions.Options;

namespace IdentityTestConsole.Services;

public class ApiTestService : IApiTestService
{
    private readonly HttpClient _httpClient;
    private readonly TestSettings _settings;

    public ApiTestService(IHttpClientFactory factory, IOptions<TestSettings> options)
    {
        _httpClient = factory.CreateClient();
        _settings = options.Value;
    }

    public async Task CallProtectedEndpointAsync(string accessToken)
    {
        Console.WriteLine("🔐 Calling protected Department API...");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync($"{_settings.ResourceServiceBaseUrl}/departments");

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("✅ Access to protected endpoint succeeded.");
            var json = await response.Content.ReadAsStringAsync();
            var departments = JsonSerializer.Deserialize<List<DepartmentDto>>(json);
            Console.WriteLine($"📄 Response data: {departments?.Count ?? 0} department(s) found.");
        }
        else
        {
            Console.WriteLine($"Failed to access protected resource: {response.StatusCode}");
        }
    }
}