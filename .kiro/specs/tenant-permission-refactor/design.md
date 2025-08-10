# Design Document

## Overview

This design outlines the refactoring of the multi-tenant architecture to remove subscription features and implement dedicated permissions for tenant user operations. The refactor will simplify the tenant model and establish clear permission-based access control using the existing architecture patterns.

## Architecture

### Current State Analysis

The current tenant system includes:

- Tenant entity with subscription fields (SubscriptionExpiresAt)
- Tenant user management without dedicated permissions
- Generic permission system not specifically tailored for tenant operations
- Configuration with subscription-related settings

### Target State

The refactored system will have:

- Simplified Tenant entity without subscription fields
- Dedicated TENANT_USER resource with specific actions
- Permission providers for all tenant user endpoints
- Streamlined configuration focused on tenant management

## Components and Interfaces

### 1. Database Schema Changes

#### Tenant Entity Modifications

```csharp
public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    // Remove: DateTime? SubscriptionExpiresAt

    // Navigation properties remain unchanged
    public virtual ICollection<TenantAsn> TenantAsns { get; set; } = new List<TenantAsn>();
    public virtual ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
}
```

#### Migration Requirements

- Create migration to remove SubscriptionExpiresAt column from Tenants table
- Update any existing data that depends on subscription fields

### 2. Permission System Design

#### ITenantPermissionProvider Interface

Create a dedicated interface for tenant permissions that follows the same pattern as IPermissionProvider:

```csharp
namespace MeUi.Application.Interfaces;

public interface ITenantPermissionProvider
{
    static abstract string Permission { get; }
}
```

#### Permission Scanning and Seeding

The database seeder will scan for classes implementing ITenantPermissionProvider, similar to how it currently scans for IPermissionProvider implementations:

```csharp
private async Task SeedTenantPermissionsAsync(CancellationToken ct)
{
    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
        .ToList();

    var tenantPermissionProviders = new List<Type>();

    foreach (var assembly in assemblies)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                       t.GetInterfaces().Any(i => i == typeof(ITenantPermissionProvider)))
            .ToList();

        tenantPermissionProviders.AddRange(types);
    }

    // Extract permissions and seed them following the same pattern as global permissions
}
```

### 3. Permission Provider Implementation

#### Endpoint Permission Providers

Each tenant user endpoint will implement ITenantPermissionProvider using the same pattern:

```csharp
public class CreateTenantUserEndpoint : Endpoint<CreateTenantUserRequest, ApiResponse<Guid>>, ITenantPermissionProvider
{
    public static string Permission => "CREATE:TENANT_USER";

    // Endpoint implementation...
}

public class GetTenantUsersEndpoint : Endpoint<GetTenantUsersRequest, ApiResponse<PaginatedResult<TenantUserDto>>>, ITenantPermissionProvider
{
    public static string Permission => "READ:TENANT_USER";

    // Endpoint implementation...
}
```

### 4. Configuration Updates

#### Remove Subscription Settings

```json
{
  "Tenant": {
    "EnableTenantIsolation": true,
    "MaxTenantsPerSuperAdmin": 100,
    // Remove: "DefaultTenantName": "Default Organization",
    // Remove: "DefaultSubscriptionDurationDays": 365,
    "RequireTenantContext": true
  }
}
```

### 5. Database Seeder Updates

#### Remove Tenant Seeding

Remove all tenant seeding logic from the DatabaseSeeder since tenants will be created through the application interface rather than seeded data.

## Data Models

### Updated Tenant Model

The Tenant entity will be simplified by removing subscription-related fields:

- Remove `SubscriptionExpiresAt` property
- Maintain all other existing properties and relationships
- Keep navigation properties to TenantAsns and TenantUsers

### Permission Model Extensions

The existing Permission, Resource, and Action entities will be extended with tenant user specific entries:

- New Resource: "TENANT_USER"
- New Actions: CREATE, READ, UPDATE, DELETE, ASSIGN_ROLE, REMOVE_ROLE, SUSPEND, UNSUSPEND, PROMOTE_TO_ADMIN, DEMOTE_FROM_ADMIN
- New Permissions: Combinations of the above actions and resource

## Error Handling

### Permission Validation

- Use existing authorization middleware to validate tenant user permissions
- Return 403 Forbidden for insufficient permissions
- Return 401 Unauthorized for missing authentication
- Maintain existing error response format

### Tenant Context Validation

- Continue using existing tenant context validation
- Super admins can access all tenant user operations
- Tenant admins can only access users within their tenant
- Regular users have no tenant user management permissions by default

## Testing Strategy

### Unit Tests

- Update existing tenant user endpoint tests to include permission validation
- Test permission provider implementations
- Test database seeder changes
- Test configuration changes

### Integration Tests

- Test end-to-end tenant user operations with new permission system
- Test super admin and tenant admin access patterns
- Test permission seeding during application startup
- Test database migration for subscription field removal

### Migration Testing

- Test migration rollback scenarios
- Verify data integrity after subscription field removal
- Test application startup with updated configuration
