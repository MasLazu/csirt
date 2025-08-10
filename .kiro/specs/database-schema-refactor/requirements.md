# Requirements Document

## Introduction

This feature involves refactoring the database schema to consolidate shared entities, improve multi-tenancy support, and simplify entity configurations. The changes will unify resource/action tables for tenants, consolidate password and refresh token management, rename threat intelligence entities, and standardize EF Core configurations.

## Requirements

### Requirement 1

**User Story:** As a system architect, I want to consolidate resource and action tables for tenants while maintaining permission references, so that the schema is simplified without losing functionality.

#### Acceptance Criteria

1. WHEN the system manages tenant permissions THEN it SHALL use a single unified table for resources and actions instead of separate tables
2. WHEN tenant permissions are queried THEN the system SHALL maintain existing references to resources and actions through the unified table
3. WHEN the migration is applied THEN existing tenant resource and action data SHALL be preserved and migrated to the new unified structure
4. WHEN permissions are assigned THEN the system SHALL continue to support the existing permission model that references resources and actions

### Requirement 2

**User Story:** As a system architect, I want to use a single password table with pivot tables for both regular users and tenant users, so that password management is centralized and consistent.

#### Acceptance Criteria

1. WHEN the system stores passwords THEN it SHALL use a single password table for both user types
2. WHEN a user or tenant user needs password authentication THEN the system SHALL use appropriate pivot tables to link to the centralized password table
3. WHEN password operations are performed THEN the system SHALL maintain the same security and hashing mechanisms
4. WHEN the migration is applied THEN existing password data SHALL be migrated without data loss

### Requirement 3

**User Story:** As a system architect, I want to use a single refresh token table with pivot tables for both user types, so that token management is unified and consistent.

#### Acceptance Criteria

1. WHEN the system manages refresh tokens THEN it SHALL use a single refresh token table for both regular users and tenant users
2. WHEN refresh tokens are issued or validated THEN the system SHALL use appropriate pivot tables to link users to tokens
3. WHEN token cleanup occurs THEN the system SHALL handle both user types through the unified table structure
4. WHEN the migration is applied THEN existing refresh token data SHALL be preserved and migrated correctly

### Requirement 4

**User Story:** As a developer, I want threat intelligence entities renamed to threat events, so that the naming better reflects the nature of the data being stored.

#### Acceptance Criteria

1. WHEN the system references threat data THEN it SHALL use "ThreatEvent" naming instead of "ThreatIntelligence"
2. WHEN database tables are created THEN they SHALL use threat_events naming convention
3. WHEN API endpoints are accessed THEN they SHALL continue to function with the new entity names
4. WHEN the migration is applied THEN existing threat intelligence data SHALL be renamed to threat events without data loss

### Requirement 5

**User Story:** As a developer, I want EF Core entity configurations to use default automatic naming and column types, so that configuration is simplified and follows framework conventions.

#### Acceptance Criteria

1. WHEN entity configurations are defined THEN they SHALL rely on EF Core's default naming conventions where possible
2. WHEN database tables are created THEN they SHALL use EF Core's automatic column type mapping
3. WHEN custom configuration is needed THEN it SHALL only be specified for non-standard requirements
4. WHEN the system builds THEN all entity configurations SHALL be valid and functional with the simplified approach
