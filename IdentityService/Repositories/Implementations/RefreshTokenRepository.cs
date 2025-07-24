using IdentityService.Data;
using IdentityService.Dtos;
using IdentityService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories.Implementations;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task StoreAsync(Guid userId, string token, string ipAddress, int expireInDays)
    {
        // Optionally revoke existing valid tokens
        var existingTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var rt in existingTokens)
        {
            rt.IsRevoked = true;
        }

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            IpAddress = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(expireInDays),
            IsRevoked = false
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task<bool> ValidateAsync(Guid userId, string token)
    {
        var existing = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                rt.UserId == userId &&
                rt.Token == token &&
                !rt.IsRevoked &&
                rt.ExpiresAt > DateTime.UtcNow);

        return existing != null;
    }

    public async Task RevokeAsync(Guid userId, string token)
    {
        var match = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                rt.UserId == userId &&
                rt.Token == token &&
                !rt.IsRevoked);

        if (match != null)
        {
            match.IsRevoked = true;
        }
    }
}
