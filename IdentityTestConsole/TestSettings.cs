namespace IdentityTestConsole;

public class TestSettings
{
    public string IdentityServiceBaseUrl { get; set; } = default!;
    public string ResourceServiceBaseUrl { get; set; } = default!;
    public JwtSettings Jwt { get; set; } = new();
}