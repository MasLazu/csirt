# Implementation Plan

- [x] 1. Create ITenantPermissionProvider interface

  - Create interface in MeUi.Application.Interfaces following the same pattern as IPermissionProvider
  - Define static abstract string Permission property
  - _Requirements: 2.1, 3.1_

- [x] 2. Create database migration to remove subscription fields

  - Generate migration to remove SubscriptionExpiresAt column from Tenants table
  - Test migration up and down operations
  - _Requirements: 1.1, 1.3_

- [x] 3. Update Tenant entity to remove subscription fields

  - Remove SubscriptionExpiresAt property from Tenant domain entity
  - Update any related configurations or mappings
  - _Requirements: 1.1_

- [x] 4. Update tenant user endpoints to implement ITenantPermissionProvider

  - [x] 4.1 Update CreateTenantUserEndpoint

    - Add ITenantPermissionProvider implementation
    - Define Permission as "CREATE:TENANT_USER"
    - _Requirements: 2.2, 3.2_

  - [x] 4.2 Update GetTenantUsersEndpoint

    - Add ITenantPermissionProvider implementation
    - Define Permission as "READ:TENANT_USER"
    - _Requirements: 2.2, 3.2_

  - [x] 4.3 Update GetTenantUserByIdEndpoint

    - Add ITenantPermissionProvider implementation
    - Define Permission as "READ:TENANT_USER"
    - _Requirements: 2.2, 3.2_

  - [x] 4.4 Update UpdateTenantUserEndpoint

    - Add ITenantPermissionProvider implementation
    - Define Permission as "UPDATE:TENANT_USER"
    - _Requirements: 2.2, 3.2_

  - [x] 4.5 Update DeleteTenantUserEndpoint

    - Add ITenantPermissionProvider implementation
    - Define Permission as "DELETE:TENANT_USER"
    - _Requirements: 2.2, 3.2_

  - [x] 4.6 Update AssignRoleToTenantUserEndpoint

    - Add ITenantPermissionProvider implementation
    - Define Permission as "ASSIGN_ROLE:TENANT_USER"
    - _Requirements: 2.2, 3.2_

  - [x] 4.7 Update RemoveRoleFromTenantUserEndpoint

    - Add ITenantPermissionProvider implementation
    - Define Permission as "REMOVE_ROLE:TENANT_USER"
    - _Requirements: 2.2, 3.2_

  - [x] 4.8 Update PromoteToSuperAdminEndpoint

    - Add ITenantPermissionProvider implementation
    - Define Permission as "PROMOTE_TO_ADMIN:TENANT_USER"
    - _Requirements: 2.2, 3.2_

  - [x] 4.9 Update DemoteFromSuperAdminEndpoint
    - Add ITenantPermissionProvider implementation
    - Define Permission as "DEMOTE_FROM_ADMIN:TENANT_USER"
    - _Requirements: 2.2, 3.2_

- [x] 5. Update DatabaseSeeder to scan ITenantPermissionProvider implementations

  - Add SeedTenantPermissionsAsync method following the same pattern as SeedPermissionsAsync
  - Scan for ITenantPermissionProvider implementations
  - Create resources, actions, and permissions for tenant user operations
  - _Requirements: 2.3, 3.3_

- [x] 6. Remove tenant seeding from DatabaseSeeder

  - Remove SeedTenantsAsync method and related tenant seeding logic
  - Remove tenant seeding calls from main SeedAsync method
  - _Requirements: 1.2_

- [x] 7. Update configuration files to remove subscription settings

  - Remove DefaultSubscriptionDurationDays from appsettings.json
  - Remove DefaultTenantName from appsettings.json
  - Update appsettings.Development.json accordingly
  - _Requirements: 1.4_

- [ ] 8. Update unit tests for tenant user endpoints

  - Update existing endpoint tests to include permission validation
  - Test ITenantPermissionProvider implementations
  - _Requirements: 4.1, 4.2_

- [ ] 9. Test database migration and permission seeding
  - Test migration rollback scenarios
  - Verify tenant user permissions are properly seeded
  - Test application startup with updated configuration
  - _Requirements: 1.3, 2.3, 3.3_
