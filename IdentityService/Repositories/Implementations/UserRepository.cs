using IdentityService.Data;
using IdentityService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedService.Entities;

namespace IdentityService.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task CreateUserAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}