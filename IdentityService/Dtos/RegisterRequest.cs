namespace IdentityService.Dtos;

public class RegisterRequest
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Designation { get; set; }
    public string Password { get; set; }
    public Guid RoleId { get; set; }
}
