using Microsoft.EntityFrameworkCore;
using SharedService.Entities;
using SharedService.Entities.JoinEntities;

namespace SharedService.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<RolePermissionRole> RolePermissionRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite key for join entity
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
    }
}