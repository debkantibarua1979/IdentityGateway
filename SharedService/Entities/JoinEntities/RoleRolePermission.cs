namespace SharedService.Entities.JoinEntities;

public class RoleRolePermission
{
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }
    public Guid RolePermissionId { get; set; } 
    public RolePermission? RolePermission { get; set; }
}
