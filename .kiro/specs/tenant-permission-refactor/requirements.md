# Requirements Document

## Introduction

This specification outlines the refactoring of the multi-tenant architecture to remove subscription-based features and implement a dedicated permission system for tenant users. The goal is to simplify the tenant model by removing subscription complexity and establish a clear permission-based access control system specifically for tenant operations.

## Requirements

### Requirement 1: Remove Subscription Features

**User Story:** As a system administrator, I want to remove subscription-based features from the tenant system, so that the architecture is simplified and focused on permission-based access control.

#### Acceptance Criteria

1. WHEN the system is refactored THEN the Tenant entity SHALL NOT contain subscription-related fields
2. WHEN tenant operations are performed THEN subscription validation SHALL NOT be required
3. WHEN database migrations are applied THEN subscription-related columns SHALL be removed from tenant tables
4. WHEN configuration is updated THEN subscription-related settings SHALL be removed from appsettings

### Requirement 2: Implement Dedicated Tenant User Permissions

**User Story:** As a system architect, I want dedicated resources, actions, and permissions for tenant user operations, so that tenant user access control is clearly defined and manageable.

#### Acceptance Criteria

1. WHEN the permission system is implemented THEN there SHALL be a dedicated "TENANT_USER" resource
2. WHEN tenant user operations are performed THEN specific actions SHALL be defined (CREATE, READ, UPDATE, DELETE, ASSIGN_ROLE, REMOVE_ROLE, SUSPEND, UNSUSPEND)
3. WHEN permissions are seeded THEN tenant user permissions SHALL be automatically created
4. WHEN authorization is checked THEN tenant user operations SHALL use the dedicated permission system
5. WHEN super admins access the system THEN they SHALL have all tenant user permissions across all tenants
6. WHEN tenant admins access the system THEN they SHALL have tenant user permissions only within their tenant scope

### Requirement 3: Update Permission Provider System

**User Story:** As a developer, I want tenant user endpoints to implement IPermissionProvider, so that permissions are automatically discovered and seeded during application startup.

#### Acceptance Criteria

1. WHEN tenant user endpoints are implemented THEN they SHALL implement IPermissionProvider interface
2. WHEN the application starts THEN tenant user permissions SHALL be automatically discovered
3. WHEN database seeding occurs THEN tenant user permissions SHALL be created in the database
4. WHEN permission validation occurs THEN it SHALL use the standardized permission format "action:resource"

### Requirement 4: Follow Existing Architecture Patterns

**User Story:** As a developer, I want the refactored system to follow the existing architecture patterns, so that it integrates seamlessly with the current codebase.

#### Acceptance Criteria

1. WHEN the refactor is complete THEN it SHALL use the existing repository pattern for data access
2. WHEN permissions are implemented THEN they SHALL follow the existing IPermissionProvider pattern
3. WHEN database changes are made THEN they SHALL use the existing Entity Framework migration approach
4. WHEN endpoints are updated THEN they SHALL follow the existing FastEndpoints pattern
