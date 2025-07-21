namespace IdentityService.Dtos;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; }            
    public DateTime ExpiresAt { get; set; }
    
    public bool IsRevoked { get; set; } = false; 
}