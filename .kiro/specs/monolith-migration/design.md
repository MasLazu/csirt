# Design Document

## Overview

This design document outlines the architecture for migrating the MeUi project from its current complex modular structure to a simplified monolith while transitioning from FastEndpoint CQRS to MediatR CQRS. The new architecture will maintain all existing functionality while providing better maintainability, testability, and developer experience.

## Architecture

### High-Level Architecture

The new monolith will follow a clean architecture pattern with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │  FastEndpoints  │  │   Middleware    │  │  Extensions │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │ MediatR Handlers│  │      DTOs       │  │ Behaviors   │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                     Domain Layer                            │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │    Entities     │  │   Interfaces    │  │   Services  │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                        │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │  Repositories   │  │   DbContext     │  │  Services   │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### Project Structure

The new simplified project structure will consist of 6 main projects:

```
src/
├── MeUi.Api/                          # Presentation Layer (FastEndpoints)
│   ├── Endpoints/                     # FastEndpoint controllers
│   │   ├── Authentication/            # Auth endpoints
│   │   ├── Authorization/             # Authorization endpoints
│   │   └── Users/                     # User management endpoints
│   ├── Extensions/                    # Service registration extensions
│   ├── Middleware/                    # Custom middleware
│   └── Program.cs                     # Application entry point
├── MeUi.Application/                  # Application Layer (MediatR)
│   ├── Authentication/                # Auth commands/queries/handlers
│   │   ├── Commands/                  # Authentication commands
│   │   ├── Queries/                   # Authentication queries
│   │   └── Handlers/                  # MediatR handlers
│   ├── Authorization/                 # Authorization logic
│   ├── Users/                         # User management logic
│   ├── Common/                        # Shared application logic
│   │   ├── Behaviors/                 # MediatR pipeline behaviors
│   │   ├── DTOs/                      # Data transfer objects
│   │   ├── Exceptions/                # Application exceptions
│   │   ├── Interfaces/                # Application interfaces
│   │   └── Mappings/                  # Mapster mapping configurations
│   └── DependencyInjection.cs        # Application service registration
├── MeUi.Domain/                       # Domain Layer
│   ├── Authentication/                # Auth domain entities
│   ├── Authorization/                 # Authorization domain entities
│   ├── Users/                         # User domain entities
│   ├── Common/                        # Shared domain logic
│   │   ├── Entities/                  # Base entities
│   │   ├── Interfaces/                # Domain interfaces
│   │   └── Specifications/            # Domain specifications
│   └── Shared/                        # Cross-domain shared logic
├── MeUi.Infrastructure/               # Infrastructure Layer
│   ├── Data/                          # Database related
│   │   ├── Contexts/                  # DbContext implementations
│   │   ├── Configurations/            # Entity configurations
│   │   ├── Repositories/              # Repository implementations
│   │   └── Migrations/                # EF migrations
│   ├── Services/                      # External services
│   │   ├── Authentication/            # JWT service, etc.
│   │   └── Email/                     # Email services (future)
│   └── DependencyInjection.cs        # Infrastructure service registration
├── MeUi.Shared/                       # Shared utilities
│   ├── Constants/                     # Application constants
│   ├── Extensions/                    # Extension methods
│   └── Utilities/                     # Helper utilities
└── MeUi.Contracts/                    # API contracts
    ├── Authentication/                # Auth request/response models
    ├── Authorization/                 # Authorization contracts
    ├── Users/                         # User contracts
    └── Common/                        # Shared contracts
```

## Components and Interfaces

### MediatR Integration

#### Command/Query Pattern

```csharp
// Commands
public record CreateUserCommand(string Username, string Email, string Name) : IRequest<Guid>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        // Implementation
    }
}

// Queries
public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        // Implementation
    }
}
```

#### Pipeline Behaviors

```csharp
// Validation Behavior
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        // Validation logic
    }
}

// Logging Behavior
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        // Logging logic
    }
}

// Transaction Behavior
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        // Transaction management
    }
}
```

### FastEndpoint Integration

#### Endpoint Implementation

```csharp
public class CreateUserEndpoint : Endpoint<CreateUserRequest, ApiResponse<Guid>>
{
    private readonly IMediator _mediator;

    public override void Configure()
    {
        Post("/api/users");
        AllowAnonymous(); // or configure authorization
        Description(x => x.WithTags("Users"));
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var command = new CreateUserCommand(req.Username, req.Email, req.Name);
        var result = await _mediator.Send(command, ct);

        await SendOkAsync(new ApiResponse<Guid> { Data = result, Success = true }, ct);
    }
}
```

### Repository Pattern

#### Interface Definition

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<IEnumerable<User>> GetUsersWithRolesAsync(CancellationToken ct = default);
}
```

#### Implementation

```csharp
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    // Additional implementations
}
```

## Data Models

### Domain Entities

#### User Entity

```csharp
public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<UserLoginMethod> LoginMethods { get; set; } = new List<UserLoginMethod>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
```

#### Base Entity

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
}
```

### DTOs and Contracts

#### User DTOs

```csharp
public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string Name,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateUserRequest(
    string Username,
    string Email,
    string Name
);

public record UpdateUserRequest(
    Guid Id,
    string Username,
    string Email,
    string Name
);
```

#### Mapster Configuration

```csharp
public static class MappingConfig
{
    public static void Configure()
    {
        // User mappings
        TypeAdapterConfig<User, UserDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // Command to Entity mappings
        TypeAdapterConfig<CreateUserCommand, User>
            .NewConfig()
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.IsActive, src => true)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt);
    }
}
```

### Database Context

#### Unified DbContext

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Authentication
    public DbSet<User> Users { get; set; }
    public DbSet<LoginMethod> LoginMethods { get; set; }
    public DbSet<UserLoginMethod> UserLoginMethods { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Password> Passwords { get; set; }

    // Authorization
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

## Error Handling

### Exception Hierarchy

```csharp
public abstract class ApplicationException : Exception
{
    protected ApplicationException(string message) : base(message) { }
    protected ApplicationException(string message, Exception innerException) : base(message, innerException) { }
}

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : ApplicationException
{
    public ValidationException(string message) : base(message) { }
    public ValidationException(IEnumerable<string> errors) : base(string.Join("; ", errors)) { }
}

public class ConflictException : ApplicationException
{
    public ConflictException(string message) : base(message) { }
}
```

### Global Exception Handler

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var response = exception switch
        {
            NotFoundException => new ApiResponse { Success = false, Message = exception.Message },
            ValidationException => new ApiResponse { Success = false, Message = exception.Message },
            ConflictException => new ApiResponse { Success = false, Message = exception.Message },
            _ => new ApiResponse { Success = false, Message = "An error occurred" }
        };

        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            ConflictException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, ct);

        return true;
    }
}
```

## Testing Strategy

### Unit Testing Structure

```
tests/
├── MeUi.Application.Tests/           # Application layer tests
│   ├── Authentication/               # Auth handler tests
│   ├── Authorization/                # Authorization tests
│   ├── Users/                        # User management tests
│   └── Common/                       # Shared test utilities
├── MeUi.Domain.Tests/                # Domain layer tests
│   ├── Entities/                     # Entity tests
│   └── Specifications/               # Specification tests
├── MeUi.Infrastructure.Tests/        # Infrastructure tests
│   ├── Repositories/                 # Repository tests
│   └── Services/                     # Service tests
└── MeUi.Api.Tests/                   # Integration tests
    ├── Endpoints/                    # Endpoint tests
    └── Common/                       # Test utilities
```

### Testing Patterns

```csharp
// Handler Unit Test Example
public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUserId()
    {
        // Arrange
        var command = new CreateUserCommand("testuser", "test@example.com", "Test User");
        var expectedUserId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = expectedUserId });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedUserId, result);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

## Migration Strategy

### Phase 1: Project Structure Setup

1. Create new simplified project structure
2. Set up MediatR and FastEndpoints integration
3. Configure dependency injection
4. Set up unified database context

### Phase 2: Domain Migration

1. Migrate domain entities to new Domain project
2. Create repository interfaces
3. Set up base entity and common domain logic

### Phase 3: Application Layer Migration

1. Convert existing command handlers to MediatR handlers
2. Convert query handlers to MediatR handlers
3. Implement pipeline behaviors
4. Create DTOs and mapping profiles

### Phase 4: Infrastructure Migration

1. Migrate database contexts to unified context
2. Implement repository pattern
3. Migrate external services
4. Update database migrations

### Phase 5: Presentation Layer Migration

1. Convert FastEndpoint controllers to use MediatR
2. Update middleware and extensions
3. Configure global exception handling
4. Update Program.cs

### Phase 6: Testing and Validation

1. Create comprehensive test suite
2. Validate all existing functionality
3. Performance testing
4. Documentation updates

## Performance Considerations

### MediatR Optimization

- Use source generators for handler registration
- Implement caching for frequently accessed data
- Use async/await patterns consistently
- Consider using MediatR.Extensions.Microsoft.DependencyInjection for better performance

### Database Optimization

- Maintain existing Entity Framework optimizations
- Use compiled queries where appropriate
- Implement proper indexing strategies
- Consider connection pooling optimization

### Memory Management

- Use record types for DTOs to reduce memory allocation
- Implement proper disposal patterns
- Use object pooling for frequently created objects
- Monitor garbage collection performance

## Security Considerations

### Authentication Flow

- Maintain existing JWT implementation
- Ensure secure token handling in MediatR pipeline
- Implement proper authorization checks in handlers

### Authorization Integration

- Use MediatR behaviors for authorization checks
- Maintain RBAC functionality
- Implement resource-based authorization where needed

### Data Protection

- Ensure sensitive data is properly handled in DTOs
- Implement audit logging through MediatR behaviors
- Maintain encryption for sensitive fields
