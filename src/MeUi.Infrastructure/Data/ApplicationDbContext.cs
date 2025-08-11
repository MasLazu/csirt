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
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<Password> Passwords { get; set; }
    public DbSet<UserPassword> UserPasswords { get; set; }

    // Authorization entities
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Domain.Entities.Action> Actions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<TenantPermission> TenantPermissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    // Page entities
    public DbSet<PageGroup> PageGroups { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<PagePermission> PagePermissions { get; set; }

    // Tenant entities
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantAsnRegistry> TenantAsnRegistries { get; set; }
    public DbSet<TenantUser> TenantUsers { get; set; }
    public DbSet<TenantUserLoginMethod> TenantUserLoginMethods { get; set; }
    public DbSet<TenantUserRefreshToken> TenantUserRefreshTokens { get; set; }
    public DbSet<TenantUserRole> TenantUserRoles { get; set; }
    public DbSet<TenantUserPassword> TenantUserPasswords { get; set; }

    // TimescaleDB entities
    public DbSet<ThreatEvent> ThreatEvents { get; set; }
    public DbSet<AsnRegistry> AsnRegistries { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Protocol> Protocols { get; set; }
    public DbSet<MalwareFamily> MalwareFamilies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ConfigurePostgreSqlSettings(modelBuilder);
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    private static void ConfigurePostgreSqlSettings(ModelBuilder modelBuilder)
    {
        // Configure PostgreSQL-specific optimizations for TimescaleDB
        // These settings help optimize EF Core for PostgreSQL/TimescaleDB usage

        // Note: PostgreSQL extensions (timescaledb, uuid-ossp, etc.) are enabled at database level
        // The migration handles TimescaleDB-specific configurations like hypertables and policies
        // Default schema is already "public" in PostgreSQL, no need to explicitly set it
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Configure PostgreSQL-specific options for TimescaleDB optimization
        if (!optionsBuilder.IsConfigured)
        {
            // This will only be used if no connection string is provided via DI
            // Mainly for design-time operations
            return;
        }

        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
        
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        // User and Authentication entities (inherit from BaseEntity)
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<LoginMethod>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<UserLoginMethod>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<UserRefreshToken>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<Password>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<UserPassword>().HasQueryFilter(e => !e.DeletedAt.HasValue);

        // Authorization entities (inherit from BaseEntity)
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<UserRole>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<Resource>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<Domain.Entities.Action>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<Permission>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<TenantPermission>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<RolePermission>().HasQueryFilter(e => !e.DeletedAt.HasValue);

        // Page entities (inherit from BaseEntity)
        modelBuilder.Entity<PageGroup>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<Page>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<PagePermission>().HasQueryFilter(e => !e.DeletedAt.HasValue);

        // Tenant entities (inherit from BaseEntity)
        modelBuilder.Entity<Tenant>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<TenantAsnRegistry>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<TenantUser>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<TenantUserLoginMethod>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<TenantUserRefreshToken>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<TenantUserRole>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        modelBuilder.Entity<TenantUserPassword>().HasQueryFilter(e => !e.DeletedAt.HasValue);

        modelBuilder.Entity<ThreatEvent>().HasQueryFilter(e => !e.DeletedAt.HasValue);
    }
}