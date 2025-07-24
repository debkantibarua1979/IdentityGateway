using IdentityService.Data;
using IdentityService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedService.Entities;

namespace IdentityService.Repositories.Implementations;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly AppDbContext _context;

    public RolePermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RolePermission?> GetByIdAsync(Guid id)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Parent)
            .Include(rp => rp.Children)
            .FirstOrDefaultAsync(rp => rp.Id == id);
    }

    public async Task<List<RolePermission>> GetAllAsync()
    {
        return await _context.RolePermissions.ToListAsync();
    }

    public async Task AddAsync(RolePermission rolePermission)
    {
        await _context.RolePermissions.AddAsync(rolePermission);
    }

    public async Task<List<RolePermission>> GetAllWithHierarchyAsync()
    {
        return await _context.RolePermissions
            .Include(rp => rp.Parent)
            .Include(rp => rp.Children)
            .ToListAsync();
    }

    public async Task<List<RolePermission>> GetAncestorsAsync(Guid permissionId)
    {
        var result = new List<RolePermission>();
        var visited = new HashSet<Guid>();

        var current = await _context.RolePermissions
            .Include(rp => rp.Parent)
            .FirstOrDefaultAsync(rp => rp.Id == permissionId);

        while (current?.Parent != null && !visited.Contains(current.Parent.Id))
        {
            result.Add(current.Parent);
            visited.Add(current.Parent.Id);

            current = await _context.RolePermissions
                .Include(rp => rp.Parent)
                .FirstOrDefaultAsync(rp => rp.Id == current.Parent.Id);
        }

        return result;
    }

    public async Task<List<RolePermission>> GetDescendantsAsync(Guid permissionId)
    {
        var descendants = new List<RolePermission>();
        var stack = new Stack<Guid>();
        stack.Push(permissionId);

        while (stack.Count > 0)
        {
            var currentId = stack.Pop();

            var children = await _context.RolePermissions
                .Where(rp => rp.ParentId == currentId)
                .ToListAsync();

            foreach (var child in children)
            {
                descendants.Add(child);
                stack.Push(child.Id);
            }
        }

        return descendants;
    }
}

