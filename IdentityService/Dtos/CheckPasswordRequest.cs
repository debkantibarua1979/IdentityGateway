namespace IdentityService.Dtos;

public class CheckPasswordRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; 
}