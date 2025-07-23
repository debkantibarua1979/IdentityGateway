using System.Text;
using IdentityService.Data;
using IdentityService.Dtos;
using IdentityService.Middlewares;
using IdentityService.Repositories.Implementations;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Implementations;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Responder;

var builder = WebApplication.CreateBuilder(args);

// Load JWT settings
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
var jwtOptions = jwtSection.Get<JwtOptions>();
var key = Encoding.UTF8.GetBytes(jwtOptions.Key);

// DbContext (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Controllers
builder.Services.AddControllers();

// Register Services & Repositories
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

// Register Middleware
builder.Services.AddScoped<TokenExpirationMiddleware>();

// Register Custom IHttpResponder (for Ocelot 24+ compatibility)
builder.Services.AddSingleton<IHttpResponder, SafeHttpResponder>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Add Ocelot (using ocelot.json)
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// Swagger (optional for debugging)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Token expiration check middleware
app.UseMiddleware<TokenExpirationMiddleware>();

app.MapControllers();

// Ocelot gateway
await app.UseOcelot();

app.Run();
