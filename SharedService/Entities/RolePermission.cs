using SharedService.Entities.JoinEntities;

namespace SharedService.Entities;


public class RolePermission
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }

    public RolePermission? Parent { get; set; }

    public ICollection<RolePermission> Children { get; set; } = new List<RolePermission>();

    public ICollection<RoleRolePermission> RoleRolePermissions { get; set; } = new List<RoleRolePermission>();
}
