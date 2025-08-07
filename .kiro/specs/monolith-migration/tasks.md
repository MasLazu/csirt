# Implementation Plan

- [x] 1. Set up new project structure and core infrastructure

  - Create the 6 main projects (Api, Application, Domain, Infrastructure, Shared, Contracts)
  - Configure project references and dependencies
  - Set up basic folder structure within each project
  - _Requirements: 1.1, 1.2, 1.3_

- [x] 2. Configure MediatR and core dependencies

  - Add MediatR NuGet packages to Application project
  - Add Mapster NuGet packages for object mapping
  - Configure MediatR service registration with pipeline behaviors
  - Set up Mapster configuration and registration
  - _Requirements: 2.1, 2.2, 2.3_

- [-] 3. Create domain layer foundation

  - Implement BaseEntity class with common properties
  - Create domain interfaces (IRepository, IUnitOfWork)
  - Set up domain specifications pattern
  - Create shared domain constants and enums
  - _Requirements: 4.2, 5.1_

- [x] 4. Migrate domain entities from existing modules
- [x] 4.1 Create User domain entities

  - Migrate User, LoginMethod, UserLoginMethod entities
  - Implement RefreshToken entity
  - Set up entity relationships and navigation properties
  - _Requirements: 4.2, 5.1, 5.2_

- [x] 4.2 Create Authentication domain entities

  - Migrate Password entity
  - Create authentication-related value objects
  - Implement domain validation rules
  - _Requirements: 4.2, 5.1, 5.2_

- [x] 4.3 Create Authorization domain entities

  - Migrate Role, Permission, RolePermission entities
  - Create UserRole entity
  - Implement RBAC domain logic
  - _Requirements: 4.2, 5.1, 5.2_

- [x] 5. Set up unified database context and infrastructure
- [x] 5.1 Create ApplicationDbContext

  - Implement unified DbContext with all entity sets
  - Configure entity relationships using Fluent API
  - Set up entity configurations in separate files
  - _Requirements: 4.4, 5.4_

- [x] 5.2 Implement repository pattern

  - Create generic Repository<T> base implementation
  - Implement specific repositories (UserRepository, etc.)
  - Create UnitOfWork implementation
  - Set up repository interfaces in domain layer
  - _Requirements: 4.4, 5.4_

- [x] 5.3 Migrate database configurations and seeders

  - Move entity configurations from existing modules
  - Consolidate database seeders
  - Update connection string configuration
  - _Requirements: 5.4_

- [ ] 6. Create application layer with MediatR handlers
- [x] 6.1 Set up MediatR pipeline behaviors

  - Implement ValidationBehavior for FluentValidation
  - Create LoggingBehavior for request/response logging
  - Implement TransactionBehavior for database transactions
  - Create AuthorizationBehavior for permission checks
  - _Requirements: 2.2, 2.3, 8.1, 8.2_

- [x] 6.2 Create DTOs and Mapster configurations

  - Define all DTOs as record types
  - Configure Mapster mappings for entity-to-DTO conversion
  - Set up command-to-entity mappings
  - Create mapping extension methods
  - _Requirements: 4.3, 7.1_

- [x] 6.3 Implement User management commands and handlers

  - Create CreateUserCommand and handler
  - Implement UpdateUserCommand and handler
  - Create DeleteUserCommand and handler
  - Add command validation using FluentValidation
  - _Requirements: 2.1, 2.2, 5.1_

- [x] 6.4 Implement User management queries and handlers

  - Create GetUserByIdQuery and handler
  - Implement GetUserByEmailQuery and handler
  - Create GetUsersPaginatedQuery and handler
  - Add query result mapping using Mapster
  - _Requirements: 2.1, 2.2, 5.1_

- [x] 6.5 Migrate Authentication commands and handlers

  - Convert existing authentication command handlers to MediatR
  - Implement CreateTokenPairCommand and handler
  - Create TokenRefreshCommand and handler
  - Add authentication-specific validation
  - _Requirements: 2.1, 2.2, 5.2_

- [x] 6.6 Migrate Authentication queries and handlers

  - Convert authentication query handlers to MediatR
  - Implement GetActiveLoginMethodsQuery and handler
  - Create user authentication validation queries
  - _Requirements: 2.1, 2.2, 5.2_

- [x] 6.7 Migrate Authorization commands and handlers

  - Convert RBAC command handlers to MediatR
  - Implement role and permission management commands
  - Create user role assignment commands
  - _Requirements: 2.1, 2.2, 5.2_

- [x] 6.8 Migrate Authorization queries and handlers

  - Convert authorization query handlers to MediatR
  - Implement permission checking queries
  - Create role and permission retrieval queries
  - _Requirements: 2.1, 2.2, 5.2_

- [x] 7. Migrate external services to infrastructure layer
- [x] 7.1 Migrate JWT service

  - Move JwtService to Infrastructure/Services/Authentication
  - Update interface to match new structure
  - Configure JWT service registration
  - _Requirements: 5.4, 8.3_

- [x] 7.2 Set up infrastructure service registration

  - Create DependencyInjection.cs in Infrastructure project
  - Register all repositories and services
  - Configure database context registration
  - _Requirements: 6.1, 6.2_

- [x] 8. Create presentation layer with FastEndpoints
- [x] 8.1 Set up FastEndpoints infrastructure

  - Configure FastEndpoints in new Api project
  - Set up endpoint base classes
  - Configure Swagger documentation
  - _Requirements: 3.1, 3.4_

- [x] 8.2 Create User management endpoints

  - Implement CreateUserEndpoint using MediatR
  - Create UpdateUserEndpoint with MediatR integration
  - Implement GetUserEndpoint and GetUsersEndpoint
  - Add endpoint validation and error handling
  - _Requirements: 3.1, 3.2, 3.3_

- [x] 8.3 Migrate Authentication endpoints

  - Convert existing authentication endpoints to use MediatR
  - Implement LoginEndpoint and RegisterEndpoint
  - Create TokenRefreshEndpoint
  - Maintain existing response formats
  - _Requirements: 3.1, 3.2, 3.3, 5.2_

- [x] 8.4 Migrate Authorization endpoints

  - Convert RBAC endpoints to use MediatR
  - Implement role and permission management endpoints
  - Create user role assignment endpoints
  - _Requirements: 3.1, 3.2, 3.3, 5.2_

- [-] 9. Set up global error handling and middleware
- [x] 9.1 Implement global exception handler

  - Create GlobalExceptionHandler for consistent error responses
  - Map application exceptions to HTTP status codes
  - Maintain existing error response formats
  - _Requirements: 8.1, 8.3, 8.4_

- [x] 9.2 Configure middleware pipeline

  - Set up authentication and authorization middleware
  - Configure CORS middleware
  - Add request logging middleware
  - _Requirements: 5.4, 8.4_

- [x] 10. Update application startup and configuration
- [x] 10.1 Create new Program.cs

  - Configure all services using extension methods
  - Set up MediatR with all assemblies
  - Configure FastEndpoints and Swagger
  - Add global exception handling
  - _Requirements: 6.1, 6.2, 6.3_

- [x] 10.2 Update configuration files

  - Consolidate appsettings.json files
  - Update connection strings for unified context
  - Configure JWT settings
  - _Requirements: 5.4, 6.2_

- [x] 11. Create database migrations for unified context

  - Generate initial migration for unified ApplicationDbContext
  - Create migration scripts to preserve existing data
  - Test migration on development database
  - _Requirements: 5.4_

- [ ] 12. Set up comprehensive testing infrastructure
- [ ] 12.1 Create unit test projects

  - Set up test projects for each layer
  - Configure test dependencies (xUnit, Moq, FluentAssertions)
  - Create test utilities and base classes
  - _Requirements: 7.1, 7.2, 7.4_

- [ ] 12.2 Write MediatR handler unit tests

  - Create unit tests for all command handlers
  - Implement unit tests for all query handlers
  - Test pipeline behaviors in isolation
  - _Requirements: 7.1, 7.2_

- [ ] 12.3 Create integration tests

  - Set up integration test infrastructure
  - Create endpoint integration tests
  - Test complete request flows through MediatR
  - _Requirements: 7.3_

- [ ] 13. Validate functionality and performance
- [ ] 13.1 Functional validation

  - Test all existing API endpoints
  - Verify authentication and authorization flows
  - Validate database operations and migrations
  - _Requirements: 5.1, 5.2, 5.3, 5.4_

- [ ] 13.2 Performance testing

  - Compare performance with existing implementation
  - Test MediatR handler performance
  - Validate database query performance
  - _Requirements: 7.4_

- [ ] 14. Clean up and documentation
- [ ] 14.1 Remove old project structure

  - Delete old modular projects after validation
  - Update solution file
  - Clean up unused dependencies
  - _Requirements: 1.1, 1.2_

- [ ] 14.2 Update documentation
  - Update README with new project structure
  - Document new development patterns
  - Create migration guide for developers
  - _Requirements: 1.4_
