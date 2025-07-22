using SharedService.Entities.JoinEntities;

namespace SharedService.Entities;


public class RolePermission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string PermissionName { get; set; }
    public string VisibleName { get; set; }
    
    // Parent-child hierarchy 
    public Guid? ParentId { get; set; }
    public RolePermission? Parent { get; set; }
    public ICollection<RolePermission>? Children { get; set; } = new List<RolePermission>();
    
    // Many-to-many relationship with roles
    public ICollection<RolePermissionRole>? RolePermissionRoles { get; set; }
}
