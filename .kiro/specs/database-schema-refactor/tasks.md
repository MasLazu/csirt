# Implementation Plan

- [x] 1. Create new TenantPermission entity and configuration

  - Create TenantPermission entity in MeUi.Domain.Entities
  - Add navigation properties to Resource and Action entities
  - Create TenantPermissionConfiguration using simplified EF Core conventions
  - _Requirements: 1.1, 5.1_

- [x] 2. Create centralized password management entities

  - [x] 2.1 Update Password entity to remove direct user relationship

    - Remove UserLoginMethodId property from Password entity
    - Add UserPasswords and TenantUserPasswords navigation collections
    - _Requirements: 2.1, 2.2_

  - [x] 2.2 Create UserPassword pivot entity

    - Create UserPassword entity with UserId, PasswordId, and UserLoginMethodId
    - Add navigation properties to User, Password, and UserLoginMethod
    - Create UserPasswordConfiguration using simplified EF Core conventions
    - _Requirements: 2.1, 2.2_

  - [x] 2.3 Update TenantUserPassword to be a pivot entity
    - Modify TenantUserPassword to include PasswordId reference
    - Update navigation properties for pivot table structure
    - Update TenantUserPasswordConfiguration using simplified EF Core conventions
    - _Requirements: 2.1, 2.2_

- [x] 3. Create centralized refresh token management entities

  - [x] 3.1 Update RefreshToken entity to remove direct user relationship

    - Remove UserId property from RefreshToken entity
    - Add UserRefreshTokens and TenantUserRefreshTokens navigation collections
    - _Requirements: 3.1, 3.2_

  - [x] 3.2 Create UserRefreshToken pivot entity

    - Create UserRefreshToken entity with UserId and RefreshTokenId
    - Add navigation properties to User and RefreshToken
    - Create UserRefreshTokenConfiguration using simplified EF Core conventions
    - _Requirements: 3.1, 3.2_

  - [x] 3.3 Update TenantUserRefreshToken to be a pivot entity
    - Modify TenantUserRefreshToken to include RefreshTokenId reference
    - Update navigation properties for pivot table structure
    - Update TenantUserRefreshTokenConfiguration using simplified EF Core conventions
    - _Requirements: 3.1, 3.2_

- [x] 4. Rename ThreatIntelligence to ThreatEvent

  - [x] 4.1 Create new ThreatEvent entity

    - Copy ThreatIntelligence entity structure to new ThreatEvent entity
    - Update all navigation properties and relationships
    - _Requirements: 4.1, 4.2_

  - [x] 4.2 Create ThreatEventConfiguration

    - Create configuration using simplified EF Core conventions
    - Remove explicit column names and types, keep only necessary constraints
    - _Requirements: 4.1, 4.2, 5.1_

  - [x] 4.3 Update IThreatIntelligenceRepository to IThreatEventRepository

    - Rename interface and update all method signatures
    - Update implementation to use ThreatEvent entity
    - _Requirements: 4.1, 4.2_

  - [x] 4.4 Update all threat intelligence queries and handlers

    - Update query classes to use ThreatEvent instead of ThreatIntelligence
    - Update query handlers to use new entity and repository
    - Update DTOs and mapping configurations
    - _Requirements: 4.1, 4.2_

  - [x] 4.5 Update threat intelligence API endpoints
    - Update all endpoint classes to use new queries and DTOs
    - Ensure API contracts remain unchanged for backward compatibility
    - _Requirements: 4.1, 4.2_

- [x] 5. Simplify all EF Core configurations

  - [x] 5.1 Update existing entity configurations

    - Remove explicit HasColumnName() calls from all configurations
    - Remove explicit HasColumnType() calls from all configurations
    - Keep only necessary constraints like IsRequired(), HasMaxLength(), etc.
    - _Requirements: 5.1_

  - [x] 5.2 Update relationship configurations
    - Simplify foreign key and navigation property configurations
    - Remove explicit column name specifications in relationships
    - _Requirements: 5.1_

- [x] 6. Create database migration

  - [x] 6.1 Generate migration for new entities and pivot tables

    - Create migration for TenantPermission table
    - Create migration for UserPassword and UserRefreshToken pivot tables
    - Create migration for ThreatEvent table (rename from ThreatIntelligence)
    - _Requirements: 1.1, 2.1, 2.2, 3.1, 3.2, 4.1, 4.2_

  - [x] 6.2 Create data migration scripts

    - Migrate existing password data to new pivot table structure
    - Migrate existing refresh token data to new pivot table structure
    - Migrate tenant-relevant permissions to TenantPermission table
    - _Requirements: 1.1, 2.3, 3.3_

  - [x] 6.3 Create cleanup migration
    - Drop old TenantUserPassword table after data migration
    - Drop old TenantUserRefreshToken table after data migration
    - Drop old ThreatIntelligence table after rename
    - _Requirements: 2.3, 3.3, 4.2_

- [x] 7. Update application services and repositories

  - [x] 7.1 Update authentication services

    - Modify password validation to use new pivot table structure
    - Update refresh token handling to use new pivot table structure
    - _Requirements: 2.3, 3.3_

  - [x] 7.2 Update tenant permission services
    - Create services to work with new TenantPermission table
    - Update existing permission queries to use appropriate table
    - _Requirements: 1.1, 1.2_

- [ ] 8. Update unit tests

  - [ ] 8.1 Update entity tests

    - Test new TenantPermission entity relationships
    - Test new pivot table entity relationships
    - Test ThreatEvent entity functionality
    - _Requirements: 1.1, 2.1, 2.2, 3.1, 3.2, 4.1_

  - [ ] 8.2 Update repository and service tests
    - Update authentication service tests for new pivot structure
    - Update threat intelligence repository tests for ThreatEvent
    - Update permission service tests for TenantPermission
    - _Requirements: 1.2, 2.3, 3.3, 4.2_

- [-] 9. Update integration tests

  - [ ] 9.1 Update API endpoint tests

    - Verify threat intelligence endpoints work with ThreatEvent
    - Verify authentication flows work with new pivot tables
    - Verify permission endpoints work with TenantPermission
    - _Requirements: 1.2, 2.3, 3.3, 4.2_

  - [ ] 9.2 Update database integration tests
    - Test migration scripts execute successfully
    - Test data integrity after migrations
    - Test performance of new table structures
    - _Requirements: 1.1, 2.1, 2.2, 3.1, 3.2, 4.1, 4.2_
