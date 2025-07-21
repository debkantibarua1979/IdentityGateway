namespace IdentityService.Dtos;

public class AccessToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public string Token { get; set; }          
    public string IpAddress { get; set; }      
    public DateTime ExpiresAt { get; set; }  
}