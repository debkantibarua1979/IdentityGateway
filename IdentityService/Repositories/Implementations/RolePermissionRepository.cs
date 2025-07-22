using IdentityService.Data;
using IdentityService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedService.Entities;
using SharedService.Entities.JoinEntities;

namespace IdentityService.Repositories.Implementations;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly AppDbContext _db;

    public RolePermissionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<RolePermission>> GetAllPermissionsAsync()
    {
        return await _db.RolePermissions.ToListAsync();
    }

    public async Task AssignPermissionsToRoleAsync(Guid roleId, IEnumerable<Guid> permissionIds)
    {
        var existing = _db.RolePermissionRoles.Where(rpr => rpr.RoleId == roleId);
        _db.RolePermissionRoles.RemoveRange(existing);

        foreach (var permissionId in permissionIds)
        {
            _db.RolePermissionRoles.Add(new RolePermissionRole
            {
                RoleId = roleId,
                RolePermissionId = permissionId
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task<List<string>> GetPermissionsByUserIdAsync(Guid userId)
    {
        var user = await _db.Users
            .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissionRoles)
                    .ThenInclude(rpr => rpr.RolePermission)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Role == null)
            return new List<string>();

        var directPermissions = user.Role.RolePermissionRoles
            .Select(rpr => rpr.RolePermission.Id)
            .ToList();

        var allPermissions = await GetPermissionsWithAncestorsAsync(directPermissions);

        return allPermissions
            .Select(p => p.PermissionName)
            .Distinct()
            .ToList();
    }

    public async Task<List<RolePermission>> GetPermissionsWithAncestorsAsync(IEnumerable<Guid> permissionIds)
    {
        var visited = new HashSet<Guid>();
        var result = new List<RolePermission>();

        foreach (var permissionId in permissionIds)
        {
            var permission = await _db.RolePermissions.FindAsync(permissionId);
            if (permission != null)
            {
                await AddWithAncestorsAsync(permission, visited, result);
            }
        }

        return result;
    }

    private async Task AddWithAncestorsAsync(RolePermission permission, HashSet<Guid> visited, List<RolePermission> result)
    {
        if (visited.Contains(permission.Id))
            return;

        visited.Add(permission.Id);
        result.Add(permission);

        if (permission.ParentId.HasValue)
        {
            var parent = await _db.RolePermissions.FindAsync(permission.ParentId.Value);
            if (parent != null)
                await AddWithAncestorsAsync(parent, visited, result);
        }
    }
}
