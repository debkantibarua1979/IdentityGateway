using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityTestConsole;
using IdentityTestConsole.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddJsonFile("test.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient();
        services.AddSingleton<IAuthTestService, AuthTestService>();
        services.AddSingleton<IApiTestService, ApiTestService>();

        services.Configure<TestSettings>(context.Configuration);
    })
    .Build();

var auth = host.Services.GetRequiredService<IAuthTestService>();
var api = host.Services.GetRequiredService<IApiTestService>();

Console.WriteLine("Testing user login + refresh + API access flow...\n");

// 1. Register (if needed)
await auth.RegisterAsync("testuser@example.com", "Test123$");

// 2. Login and get tokens
var loginResult = await auth.LoginAsync("testuser@example.com", "Test123$");

if (!loginResult.Success)
{
    Console.WriteLine("Login failed.");
    return;
}

// 3. Call secured Resource API
await api.CallProtectedEndpointAsync(loginResult.AccessToken);

// 4. Refresh token
var refreshed = await auth.RefreshTokenAsync(loginResult.RefreshToken);

if (refreshed.Success)
{
    await api.CallProtectedEndpointAsync(refreshed.AccessToken);
}
else
{
    Console.WriteLine("Refresh token failed.");
}