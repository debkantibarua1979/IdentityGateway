using System.IdentityModel.Tokens.Jwt;
using IdentityService.Data;
using Microsoft.EntityFrameworkCore;


namespace IdentityService.Middlewares;

public class TokenExpirationMiddleware : IMiddleware
{
    private readonly AppDbContext _db;

    public TokenExpirationMiddleware(AppDbContext db)
    {
        _db = db;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwt = handler.ReadJwtToken(token);
                var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var ipClaim = jwt.Claims.FirstOrDefault(c => c.Type == "ip")?.Value;
                var requestIp = context.Connection.RemoteIpAddress?.ToString();

                var storedToken = await _db.AccessTokens
                    .FirstOrDefaultAsync(t => t.Token == token && t.UserId.ToString() == userId);

                if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow || storedToken.IpAddress != requestIp)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Access token is invalid, expired, or IP-mismatched.");
                    return;
                }
            }
        }

        await next(context);
    }
}