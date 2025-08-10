# Design Document

## Overview

This design outlines the refactoring of the database schema to consolidate shared entities, improve multi-tenancy support, and simplify configurations. The changes involve eliminating tenant-specific resource/action tables, centralizing password and refresh token management, renaming threat intelligence to threat events, and standardizing EF Core configurations.

## Architecture

### Current State Analysis

The current system has:

- Global `Resource`, `Action`, and `Permission` tables
- Tenant permissions are currently filtered from the global permission table based on tenant-relevant resources
- No dedicated `TenantPermission` table for tenant-specific permissions
- Separate password tables: `Password` and `TenantUserPassword`
- Separate refresh token tables: `RefreshToken` and `TenantUserRefreshToken`
- `ThreatIntelligence` entity with custom naming
- Verbose EF Core configurations with explicit column definitions

### Target State

The refactored system will have:

- Global `Resource` and `Action` tables used by both global and tenant permissions
- Global `Permission` table for system-wide permissions
- New `TenantPermission` table for tenant-specific permissions that reference the same global resources and actions
- Single `Password` table with pivot tables for user associations
- Single `RefreshToken` table with pivot tables for user associations
- `ThreatEvent` entity replacing `ThreatIntelligence`
- Simplified EF Core configurations using framework defaults

## Components and Interfaces

### 1. Consolidated Resource and Action Usage

**Updated Global Entities**

```csharp
public class Resource : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Permission> Permissions { get; set; } = [];
    public ICollection<TenantPermission> TenantPermissions { get; set; } = [];
}

public class Action : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Permission> Permissions { get; set; } = [];
    public ICollection<TenantPermission> TenantPermissions { get; set; } = [];
}
```

**Permission Entity (No Changes)**

```csharp
public class Permission : BaseEntity
{
    public string ResourceCode { get; set; } = string.Empty;
    public string ActionCode { get; set; } = string.Empty;

    public Resource? Resource { get; set; }
    public Action? Action { get; set; }
    // ... existing collections remain
}
```

**New TenantPermission Entity**

```csharp
public class TenantPermission : BaseEntity
{
    public string ResourceCode { get; set; } = string.Empty;
    public string ActionCode { get; set; } = string.Empty;

    public Resource? Resource { get; set; }
    public Action? Action { get; set; }

    // Future: Will be used by tenant roles
    // public ICollection<TenantRolePermission> TenantRolePermissions { get; set; } = [];
}
```

### 2. Centralized Password Management

**Updated Password Entity**

```csharp
public class Password : BaseEntity
{
    public string PasswordHash { get; set; } = string.Empty;
    public string? PasswordSalt { get; set; }

    public ICollection<UserPassword> UserPasswords { get; set; } = [];
    public ICollection<TenantUserPassword> TenantUserPasswords { get; set; } = [];
}
```

**New Pivot Tables**

```csharp
public class UserPassword : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PasswordId { get; set; }
    public Guid UserLoginMethodId { get; set; }

    public User? User { get; set; }
    public Password? Password { get; set; }
    public UserLoginMethod? UserLoginMethod { get; set; }

    // Computed properties for backward compatibility
    public string PasswordHash => Password?.PasswordHash ?? string.Empty;
    public string? PasswordSalt => Password?.PasswordSalt;
}

public class TenantUserPassword : BaseEntity
{
    public Guid TenantUserId { get; set; }
    public Guid PasswordId { get; set; }
    public Guid TenantUserLoginMethodId { get; set; }

    public TenantUser? TenantUser { get; set; }
    public Password? Password { get; set; }
    public TenantUserLoginMethod? TenantUserLoginMethod { get; set; }

    // Computed properties for backward compatibility
    public string PasswordHash => Password?.PasswordHash ?? string.Empty;
    public string? PasswordSalt => Password?.PasswordSalt;
}
```

**✅ Implemented Password Management Architecture Benefits:**

This centralized password management architecture:

- ✅ **Eliminates password duplication** - Single Password entity shared across users
- ✅ **Uses simplified EF Core conventions** - No explicit column names/types in configurations
- ✅ **Maintains backward compatibility** - Computed properties provide seamless access to password data
- ✅ **Follows the generic repository pattern** - Uses IRepository<T> for all data access
- ✅ **Supports both regular users and tenant users** - Unified approach with separate pivot tables

### 3. Centralized Refresh Token Management

**Updated RefreshToken Entity**

```csharp
public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = [];
    public ICollection<TenantUserRefreshToken> TenantUserRefreshTokens { get; set; } = [];
}
```

**New Pivot Tables**

```csharp
public class UserRefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RefreshTokenId { get; set; }

    public User? User { get; set; }
    public RefreshToken? RefreshToken { get; set; }
}

public class TenantUserRefreshToken : BaseEntity
{
    public Guid TenantUserId { get; set; }
    public Guid RefreshTokenId { get; set; }

    public TenantUser? TenantUser { get; set; }
    public RefreshToken? RefreshToken { get; set; }
}
```

### 4. Threat Event Entity

**Renamed Entity: `ThreatEvent`**

```csharp
public class ThreatEvent : BaseEntity
{
    public DateTime Timestamp { get; set; }
    public Guid AsnId { get; set; }
    public IPAddress SourceAddress { get; set; } = IPAddress.None;
    public Guid? SourceCountryId { get; set; }
    public IPAddress? DestinationAddress { get; set; }
    public Guid? DestinationCountryId { get; set; }
    public int? SourcePort { get; set; }
    public int? DestinationPort { get; set; }
    public Guid? ProtocolId { get; set; }
    public string Category { get; set; } = string.Empty;
    public Guid? MalwareFamilyId { get; set; }

    // Navigation properties remain the same
    public virtual AsnInfo AsnInfo { get; set; } = null!;
    public virtual Country? SourceCountry { get; set; }
    public virtual Country? DestinationCountry { get; set; }
    public virtual Protocol? Protocol { get; set; }
    public virtual MalwareFamily? MalwareFamily { get; set; }
}
```

## Data Models

### Migration Strategy

**Phase 1: Create New Tables and Pivot Tables**

1. Create new `TenantPermission` table for tenant-specific permissions
2. Create new pivot tables for passwords: `UserPassword` and `TenantUserPassword`
3. Create new pivot tables for refresh tokens: `UserRefreshToken` and `TenantUserRefreshToken`
4. Migrate existing password data to centralized `Password` table and create pivot relationships
5. Migrate existing refresh token data to centralized `RefreshToken` table and create pivot relationships

**Phase 2: Update Entity References and Data Migration**

1. Update user entities to use new pivot tables for password and token relationships
2. Migrate tenant-relevant permissions from global `Permission` table to new `TenantPermission` table
3. Update `Resource` and `Action` entities to include relationships with `TenantPermission`
4. Rename `ThreatIntelligence` entity to `ThreatEvent`
5. Update all references to threat intelligence in repositories, services, and APIs

**Phase 3: Cleanup Old Tables**

1. Drop old `TenantUserPassword` table (replaced by pivot)
2. Drop old `TenantUserRefreshToken` table (replaced by pivot)
3. Update indexes and constraints for new pivot tables
4. Add proper indexes for `TenantPermission` table relationships

### Data Preservation

- Existing global `Resource` and `Action` tables remain unchanged
- Tenant-relevant permissions will be migrated from global `Permission` table to new `TenantPermission` table
- Password hashes and salts will be preserved in the centralized table with proper pivot relationships
- Refresh tokens will maintain their expiration and revocation status through pivot tables
- Threat intelligence data will be renamed to threat events without data loss
- All tenant permission references to resources and actions will continue to work through the new `TenantPermission` table

## Error Handling

### Migration Rollback Strategy

- Each migration phase will include rollback scripts
- Data validation checks before and after each phase
- Backup creation before major structural changes

### Runtime Error Handling

- Graceful handling of missing pivot table relationships
- Validation of resource/action type consistency
- Token validation with new unified structure

## Testing Strategy

### Unit Tests

- Entity relationship validation
- Migration script testing
- Data integrity verification

### Integration Tests

- Authentication flow with new token structure
- Permission checking with unified resource/action model
- API endpoint functionality with renamed entities

### Performance Tests

- Query performance with new table structures
- Index effectiveness validation
- Migration performance testing

## Implementation Considerations

### EF Core Configuration Simplification

**✅ DO: Use Simplified Conventions (Recommended Approach)**

```csharp
public class UserPasswordConfiguration : IEntityTypeConfiguration<UserPassword>
{
    public void Configure(EntityTypeBuilder<UserPassword> builder)
    {
        // ✅ Keep: Primary key configuration
        builder.HasKey(x => x.Id);

        // ✅ Keep: Required properties and constraints
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.PasswordId).IsRequired();
        builder.Property(x => x.UserLoginMethodId).IsRequired();

        // ✅ Keep: Audit properties (let EF Core determine column names/types)
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.DeletedAt);

        // ✅ Keep: Indexes for performance
        builder.HasIndex(x => new { x.UserId, x.UserLoginMethodId }).IsUnique();
        builder.HasIndex(x => x.DeletedAt);

        // ✅ Keep: Relationships and foreign keys
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

**❌ DON'T: Use Explicit Column Names and Types (Verbose Approach)**

```csharp
public class VerbosePasswordConfiguration : IEntityTypeConfiguration<Password>
{
    public void Configure(EntityTypeBuilder<Password> builder)
    {
        // ❌ Remove: Explicit table names (use EF Core defaults)
        builder.ToTable("passwords");

        // ❌ Remove: Explicit column names and types
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .HasColumnType("varchar(500)")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        // ❌ Remove: Explicit index names
        builder.HasIndex(x => x.DeletedAt)
            .HasDatabaseName("ix_passwords_deleted_at");
    }
}
```

**Configuration Guidelines:**

**✅ KEEP These Configurations:**

- Primary key definitions (`HasKey`)
- Required properties (`IsRequired()`)
- String length constraints (`HasMaxLength()`)
- Unique constraints and indexes (`HasIndex`, `IsUnique()`)
- Relationships and foreign keys (`HasOne`, `WithMany`, `HasForeignKey`)
- Delete behavior (`OnDelete`)
- Audit property configurations for BaseEntity

**❌ REMOVE These Configurations:**

- Explicit table names (`ToTable()`) - Let EF Core use class names
- Explicit column names (`HasColumnName()`) - Let EF Core use property names
- Explicit column types (`HasColumnType()`) - Let EF Core determine appropriate types
- Explicit index names (`HasDatabaseName()`) - Let EF Core generate names
- Default value SQL (`HasDefaultValueSql()`) - Use C# property initializers instead
- Value generation (`ValueGeneratedOnAdd()`) - Let EF Core handle automatically

**Real Implementation Examples:**

**✅ Simplified UserPassword Configuration (Implemented)**

```csharp
public class UserPasswordConfiguration : IEntityTypeConfiguration<UserPassword>
{
    public void Configure(EntityTypeBuilder<UserPassword> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.PasswordId).IsRequired();
        builder.Property(x => x.UserLoginMethodId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => new { x.UserId, x.UserLoginMethodId }).IsUnique();
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Password).WithMany(p => p.UserPasswords).HasForeignKey(x => x.PasswordId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.UserLoginMethod).WithMany().HasForeignKey(x => x.UserLoginMethodId).OnDelete(DeleteBehavior.Cascade);
    }
}
```

**✅ Simplified Password Configuration (Implemented)**

```csharp
public class PasswordConfiguration : IEntityTypeConfiguration<Password>
{
    public void Configure(EntityTypeBuilder<Password> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);
        builder.Property(x => x.PasswordSalt).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => x.DeletedAt);
    }
}
```

### Backward Compatibility

- API endpoints will maintain existing contracts
- Service layer will abstract the new entity structure
- Gradual migration of business logic to use new entities

### Performance Optimization

- Proper indexing on pivot table foreign keys
- Query optimization for unified resource/action lookups
- Efficient joins for user-token relationships
