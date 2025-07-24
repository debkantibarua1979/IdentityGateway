using IdentityService.Data;
using IdentityService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedService.Entities;

namespace IdentityService.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RoleRolePermissions)
            .ThenInclude(rrp => rrp.RolePermission)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RoleRolePermissions)
            .ThenInclude(rrp => rrp.RolePermission)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RoleRolePermissions)
            .ThenInclude(rrp => rrp.RolePermission)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RoleRolePermissions)
            .ThenInclude(rrp => rrp.RolePermission)
            .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}