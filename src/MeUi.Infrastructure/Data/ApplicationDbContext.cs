using Microsoft.EntityFrameworkCore;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // User and Authentication entities
    public DbSet<User> Users { get; set; }
    public DbSet<LoginMethod> LoginMethods { get; set; }
    public DbSet<UserLoginMethod> UserLoginMethods { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Password> Passwords { get; set; }

    // Authorization entities
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Domain.Entities.Action> Actions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    // Page entities
    public DbSet<PageGroup> PageGroups { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<PagePermission> PagePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<User>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<LoginMethod>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<UserLoginMethod>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<Password>().HasQueryFilter(e => !e.DeletedAt.HasValue);

        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<UserRole>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<Resource>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<Domain.Entities.Action>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<Permission>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<RolePermission>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<PageGroup>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<Page>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<PagePermission>().HasQueryFilter(e => !e.DeletedAt.HasValue);
    }
}