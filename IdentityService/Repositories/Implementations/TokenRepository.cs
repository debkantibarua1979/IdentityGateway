using IdentityService.Data;
using IdentityService.Dtos;
using IdentityService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories.Implementations;

public class TokenRepository: ITokenRepository
{
    private readonly AppDbContext _db;

    public TokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task SaveAccessTokenAsync(AccessToken token)
    {
        _db.AccessTokens.Add(token);
        await _db.SaveChangesAsync();
    }

    public async Task SaveRefreshTokenAsync(RefreshToken token)
    {
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
    {
        return await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);
    }

    public async Task InvalidateRefreshTokenAsync(string refreshToken)
    {
        var existing = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (existing != null)
        {
            existing.IsRevoked = true;
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task<AccessToken?> GetAccessTokenAsync(string token)
    {
        return await _db.AccessTokens
            .FirstOrDefaultAsync(t => t.Token == token);
    }

}