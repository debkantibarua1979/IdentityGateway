using SharedService.Entities.JoinEntities;

namespace SharedService.Entities;


public class RolePermission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string PermissionName { get; set; }

    public ICollection<RolePermissionRole>? RolePermissionRoles { get; set; }
}
