using SharedService.Entities;

namespace IdentityService.Dtos;

public class RefreshToken
{
    
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public User? User { get; set; } 
}