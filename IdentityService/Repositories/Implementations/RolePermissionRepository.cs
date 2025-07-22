using IdentityService.Data;
using IdentityService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories.Implementations;

public class RolePermissionRepository: IRolePermissionRepository
{
    private readonly AppDbContext _context;

    public RolePermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetPermissionsByUserIdAsync(Guid userId)
    {
        var roles = await _context.Users
            .Where(u => u.Id == userId)
            .Include(u => u.Role)
            .ThenInclude(r => r.RolePermissionRoles)
            .ThenInclude(rpr => rpr.RolePermission)
            .Select(u => u.Role)
            .ToListAsync();

        var permissions = roles
            .SelectMany(r => r?.RolePermissionRoles ?? [])
            .Select(rpr => rpr.RolePermission?.PermissionName)
            .Where(p => !string.IsNullOrEmpty(p))
            .Distinct()
            .ToList()!;

        return permissions!;
    }
}