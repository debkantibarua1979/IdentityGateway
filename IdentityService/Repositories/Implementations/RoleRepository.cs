using IdentityService.Data;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedService.Entities;
using SharedService.Entities.JoinEntities;

namespace IdentityService.Repositories.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public RoleRepository(AppDbContext context, IRolePermissionRepository rolePermissionRepository)
    {
        _context = context;
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .Include(r => r.RoleRolePermissions)
            .ThenInclude(rrp => rrp.RolePermission)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public async Task<List<RolePermission>> GetPermissionsAsync(Guid roleId)
    {
        return await _context.RoleRolePermissions
            .Where(rrp => rrp.RoleId == roleId)
            .Include(rrp => rrp.RolePermission)
            .Select(rrp => rrp.RolePermission)
            .ToListAsync();
    }

    public async Task AssignPermissionWithAncestorsAsync(Guid roleId, RolePermission permission)
    {
        var role = await _context.Roles
            .Include(r => r.RoleRolePermissions)
            .FirstOrDefaultAsync(r => r.Id == roleId);

        if (role == null)
            throw new Exception("Role not found");

        var alreadyAssigned = role.RoleRolePermissions.Select(x => x.RolePermissionId).ToHashSet();

        // Load ancestors from service
        var ancestors = await _rolePermissionRepository.GetAncestorsAsync(permission.Id);
        var toAssign = ancestors
            .Append(permission)
            .Where(p => !alreadyAssigned.Contains(p.Id))
            .DistinctBy(p => p.Id);

        foreach (var p in toAssign)
        {
            role.RoleRolePermissions.Add(new RoleRolePermission
            {
                RoleId = roleId,
                RolePermissionId = p.Id
            });
        }
    }

    public async Task RemovePermissionAsync(Guid roleId, Guid rolePermissionId)
    {
        var relation = await _context.RoleRolePermissions
            .FirstOrDefaultAsync(rrp =>
                rrp.RoleId == roleId &&
                rrp.RolePermissionId == rolePermissionId);

        if (relation != null)
        {
            _context.RoleRolePermissions.Remove(relation);
        }
    }

    public async Task<bool> HasPermissionAsync(Guid roleId, Guid rolePermissionId)
    {
        return await _context.RoleRolePermissions
            .AnyAsync(rrp => rrp.RoleId == roleId && rrp.RolePermissionId == rolePermissionId);
    }
    
    

}