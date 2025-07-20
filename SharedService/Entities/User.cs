namespace SharedService.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Designation { get; set; }
    public string Password { get; set; } // Stored as bcrypt hash

    public Guid? RoleId { get; set; }
    public Role? Role { get; set; }
}
