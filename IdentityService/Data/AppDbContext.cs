using IdentityService.Dtos;
using Microsoft.EntityFrameworkCore;
using SharedService.Entities;
using SharedService.Entities.JoinEntities;

namespace IdentityService.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // User & Role management
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<RolePermissionRole> RolePermissionRoles { get; set; }

    // Token tracking
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Role-Permission join key
        modelBuilder.Entity<RolePermissionRole>()
            .HasKey(rpr => new { rpr.RoleId, rpr.RolePermissionId });

        modelBuilder.Entity<RolePermissionRole>()
            .HasOne(rpr => rpr.Role)
            .WithMany(r => r.RolePermissionRoles)
            .HasForeignKey(rpr => rpr.RoleId);

        modelBuilder.Entity<RolePermissionRole>()
            .HasOne(rpr => rpr.RolePermission)
            .WithMany(rp => rp.RolePermissionRoles)
            .HasForeignKey(rpr => rpr.RolePermissionId);

        // Token uniqueness
        modelBuilder.Entity<AccessToken>()
            .HasIndex(t => t.Token)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(t => t.Token)
            .IsUnique();
    }
}