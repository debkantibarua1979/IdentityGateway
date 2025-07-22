using System.Security.Cryptography;
using IdentityService.Data;
using IdentityService.Dtos;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace IdentityService.Services.Implementations;

using IdentityService.Dtos;
using SharedService.Entities;



using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using SharedService.Entities;


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IRolePermissionRepository _permRepo;
    private readonly ITokenRepository _tokenRepo;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IUserRepository userRepo,
        IRolePermissionRepository permRepo,
        ITokenRepository tokenRepo,
        IOptions<JwtOptions> jwtOptions)
    {
        _userRepo = userRepo;
        _permRepo = permRepo;
        _tokenRepo = tokenRepo;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Verify(request.Password, user.Password))
            return null;

        var rolePermissions = await _permRepo.GetPermissionsByUserIdAsync(user.Id);
        var accessToken = GenerateAccessToken(user, rolePermissions, request.IpAddress, out var expiresAt);
        var refreshToken = GenerateRefreshToken();

        await _tokenRepo.SaveAccessTokenAsync(new AccessToken
        {
            UserId = user.Id,
            Token = accessToken,
            IpAddress = request.IpAddress,
            ExpiresAt = expiresAt
        });

        await _tokenRepo.SaveRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            IpAddress = request.IpAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpireDays),
            IsRevoked = false
        });

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse?> RefreshAccessTokenAsync(RefreshRequest request)
    {
        var storedToken = await _tokenRepo.GetRefreshTokenAsync(request.RefreshToken);
        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            return null;

        var user = await _userRepo.GetByIdAsync(storedToken.UserId);
        if (user == null) return null;

        var rolePermissions = await _permRepo.GetPermissionsByUserIdAsync(user.Id);
        var newAccessToken = GenerateAccessToken(user, rolePermissions, request.IpAddress, out var expiresAt);

        await _tokenRepo.SaveAccessTokenAsync(new AccessToken
        {
            UserId = user.Id,
            Token = newAccessToken,
            IpAddress = request.IpAddress,
            ExpiresAt = expiresAt
        });

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = storedToken.Token
        };
    }

    private string GenerateAccessToken(User user, List<string> permissions, string ipAddress, out DateTime expiresAt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("ip", ipAddress)
        };

        claims.AddRange(permissions.Select(p => new Claim("permissions", p)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}

