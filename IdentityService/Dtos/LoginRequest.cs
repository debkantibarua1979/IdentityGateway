namespace IdentityService.Dtos;

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; }
}
