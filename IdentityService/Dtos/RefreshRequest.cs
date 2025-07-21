namespace IdentityService.Dtos;

public class RefreshRequest
{
    public string RefreshToken { get; set; }
    public string IpAddress { get; set; }
}
