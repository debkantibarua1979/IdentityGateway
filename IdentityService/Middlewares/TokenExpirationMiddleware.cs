using IdentityService.Repositories.Interfaces;

namespace IdentityService.Middlewares;

using System.IdentityModel.Tokens.Jwt;
using IdentityService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;



public class TokenExpirationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenExpirationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (!string.IsNullOrWhiteSpace(token))
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwt = handler.ReadJwtToken(token);
                var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var ipClaim = jwt.Claims.FirstOrDefault(c => c.Type == "ip")?.Value;
                var requestIp = context.Connection.RemoteIpAddress?.ToString();

                var tokenRepo = context.RequestServices.GetRequiredService<ITokenRepository>();
                var storedToken = await tokenRepo.GetAccessTokenAsync(token);

                if (storedToken == null ||
                    storedToken.ExpiresAt < DateTime.UtcNow ||
                    storedToken.IpAddress != requestIp)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Access token is invalid, expired, or IP-mismatched.");
                    return;
                }
            }
        }

        await _next(context);
    }
}
