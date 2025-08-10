# Implementation Plan

- [x] 1. Create core tenant domain entities and configurations

  - Create Tenant domain entity with Name, Description, ContactEmail, ContactPhone, IsActive, and SubscriptionExpiresAt properties
  - Create TenantAsn junction entity with TenantId, AsnId, AssignedAt, and AssignedByTenantUserId properties
  - Create TenantUser entity with Username, Email, Name, IsSuspended, TenantId, and IsTenantAdmin properties
  - Create Entity Framework configurations for all three new tenant entities with proper relationships
  - _Requirements: 1.1, 1.2, 1.3_

- [x] 2. Create supporting tenant user authentication entities

  - Create TenantUserLoginMethod entity to support tenant user authentication
  - Create TenantUserRefreshToken entity for tenant user token management
  - Create TenantUserRole entity for tenant user authorization
  - Create Entity Framework configurations for all tenant user authentication entities
  - _Requirements: 3.1, 3.2, 3.4_

- [x] 3. Update ApplicationDbContext for tenant entities

  - Add DbSet<Tenant> Tenants to ApplicationDbContext
  - Add DbSet<TenantAsn> TenantAsns to ApplicationDbContext
  - Add DbSet<TenantUser> TenantUsers to ApplicationDbContext
  - Add DbSets for tenant user authentication entities
  - Apply soft delete query filters for all new tenant entities
  - _Requirements: 1.1, 3.1, 5.1_

- [x] 4. Create database migration for tenant schema

  - Generate EF Core migration to add tenants table with all required columns
  - Add tenant_asns junction table with foreign keys to tenants and asn_infos
  - Add tenant_users table with foreign key to tenants
  - Add tenant user authentication tables (login methods, refresh tokens, roles)
  - Create unique constraints to prevent duplicate ASN assignments
  - Add indexes for optimal query performance on tenant-related lookups
  - _Requirements: 1.1, 2.1, 2.2, 3.1_

- [x] 6. Create tenant repository and basic CRUD operations

  - Implement ITenantRepository interface with methods from design (GetByNameAsync, GetActiveTenantsAsync, etc.)
  - Create TenantRepository with methods for tenant CRUD, ASN assignment, and queries
  - Implement GetTenantAsnsAsync and ASN assignment methods using TenantUser references
  - Add unit tests for tenant repository operations
  - _Requirements: 1.1, 1.4, 2.1, 2.2_

- [x] 7. Implement tenant user management system

  - Create ITenantUserRepository interface for tenant user operations
  - Implement TenantUserRepository with tenant-scoped user management
  - Create tenant user authentication and authorization logic using TenantUser entity
  - Implement super admin detection using IsTenantAdmin flag
  - _Requirements: 3.1, 3.2, 3.3, 3.4_

- [x] 8. Create tenant management API endpoints

  - Implement CreateTenantEndpoint for super admin tenant creation (using TenantUser with IsTenantAdmin=true)
  - Create GetTenantsEndpoint with pagination for tenant listing
  - Implement UpdateTenantEndpoint and DeleteTenantEndpoint with proper validation
  - Add unit tests for all tenant management endpoints
  - _Requirements: 1.1, 1.2, 1.4, 1.5_

- [x] 9. Implement ASN assignment API endpoints

  - Create AssignAsnToTenantEndpoint for super admin ASN assignment (using TenantUser)
  - Implement GetTenantAsnsEndpoint to list ASNs assigned to a tenant
  - Create RemoveAsnFromTenantEndpoint with conflict validation
  - Add bulk ASN assignment endpoint for efficient operations
  - Update all endpoints to use AssignedByTenantUserId instead of AssignedByUserId
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [x] 10. Create tenant user management API endpoints

  - Implement CreateTenantUserEndpoint for tenant admin user creation
  - Create GetTenantUsersEndpoint with tenant-scoped user listing
  - Implement UpdateTenantUserEndpoint and DeleteTenantUserEndpoint
  - Add tenant user role assignment endpoints using TenantUserRole entity
  - Implement super admin promotion/demotion endpoints
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [ ] 11. Implement Row Level Security (RLS) policies

  - Create PostgreSQL RLS policies for threat_intelligences table using tenant context
  - Implement database functions for setting tenant context (set_tenant_context)
  - Create RLS policies for tenant_users and other tenant-sensitive tables
  - Add database migration to enable RLS and create all security policies
  - _Requirements: 5.1, 5.2, 5.3, 5.4_

- [x] 12. Update threat intelligence repository for tenant filtering (add not replcate)

  - Modify IThreatIntelligenceRepository to include tenant-aware methods
  - Update ThreatIntelligenceRepository to work with RLS policies and tenant context
  - Implement tenant context setting in repository base class before database operations
  - Add GetTenantAsnIdsAsync and GetTenantThreatCountAsync methods
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 5.1_

- [x] 13. add threat intelligence endpoints for multi-tenancy (add not replcate)

  - Modify all existing threat intelligence endpoints to work with tenant context
  - Update query handlers to automatically apply tenant filtering through RLS
  - Ensure all threat intelligence APIs respect tenant boundaries
  - Add tenant validation to all threat intelligence operations
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 5.1, 5.2_

- [ ] 14. Update JWT token service for tenant claims

  - Modify IJwtTokenService to include tenant claims in tokens for TenantUser authentication
  - Update JwtTokenService to generate tokens with tenant context from TenantUser entity
  - Implement token generation logic that handles IsTenantAdmin flag for super admin privileges
  - Add token validation for tenant context and permissions
  - _Requirements: 4.1, 4.2, 5.1, 5.2_

- [x] 15. Create tenant-aware authentication endpoints

  - Implement TenantUser login endpoints that generate tokens with tenant context
  - Create tenant user registration and password reset functionality
  - Update authentication to work exclusively with TenantUser entities
  - Add tenant context validation to all authentication operations
  - _Requirements: 3.1, 3.2, 4.1, 4.2_

- [ ] 16. Implement comprehensive error handling for multi-tenancy

  - Create tenant-specific exception classes (TenantNotFoundException, TenantAccessDeniedException, AsnAlreadyAssignedException)
  - Update GlobalExceptionHandler to handle tenant-related errors
  - Add proper error responses for tenant access violations
  - Implement error logging with tenant context information
  - _Requirements: 5.1, 5.2, 5.3, 5.4_

- [ ] 17. Implement tenant monitoring and usage tracking

  - Create tenant usage tracking repository and services using TenantUser context
  - Implement GetTenantUsageStatsEndpoint for super admin monitoring
  - Create tenant activity logging and audit trail functionality
  - Add tenant resource usage metrics and reporting
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_

- [ ] 18. Create integration tests for multi-tenant functionality

  - Implement MultiTenantTestFixture with TenantUser entities (SuperAdmin, TenantAAdmin, TenantBUser)
  - Create integration tests for tenant data isolation using RLS policies
  - Add tests for ASN assignment workflows and conflicts using TenantUser assignments
  - Test tenant user management and authentication flows
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5_

- [x] 19. Update application startup and dependency injection

  - Register all new tenant-related services in DI container (ITenantContext, ITenantRepository, ITenantUserRepository)
  - Configure TenantMiddleware in the request pipeline
  - Update database seeding to include sample tenant data with TenantUser entities
  - Add tenant-related configuration options to appsettings
  - _Requirements: 7.1, 7.2, 7.3, 7.4_

- [ ] 20. Create tenant administration dashboard endpoints
  - Implement GetTenantDashboardEndpoint with tenant overview statistics for super admin TenantUsers
  - Create tenant health monitoring and status endpoints
  - Add tenant configuration management endpoints
  - Implement tenant backup and data export functionality
  - _Requirements: 6.1, 6.2, 6.3, 6.4_
