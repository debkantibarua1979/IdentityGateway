using SharedService.Entities.JoinEntities;

namespace SharedService.Entities;


public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public ICollection<User>? Users { get; set; }
    public ICollection<RoleRolePermission>? RoleRolePermissions { get; set; }
        = new List<RoleRolePermission>();
    
    public User? User { get; set; }
}

