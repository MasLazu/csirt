# MeUi - Modular Authentication & Authorization System

A modern .NET 9 web API built with clean architecture principles, featuring modular authentication, role-based authorization, and comprehensive user management capabilities.

## Overview

MeUi is a comprehensive backend system designed for scalable applications with robust authentication and authorization features. Built using FastEndpoints, Entity Framework Core, and PostgreSQL, it provides a solid foundation for enterprise-grade web applications with clean separation of concerns.

## Architecture

The project follows a modular monolith architecture with clear separation of concerns:

### Core Modules

- **MeUi.Shared** - Common infrastructure, domain models, and application contracts
- **MeUi.Authentication** - Authentication services with multiple providers
  - **Core** - Base authentication infrastructure and JWT handling
  - **Password** - Traditional username/password authentication
- **MeUi.Authorization** - Authorization and permission management
  - **Core** - Base authorization infrastructure
  - **Rbac** - Role-based access control implementation
- **MeUi.Entry** - Main application entry point and API configuration

### Layer Structure

Each module follows clean architecture principles with Public/Private separation:

- **Public** - External contracts, extensions, and shared components
- **Private** - Internal implementation
  - **Domain** - Business entities and domain logic
  - **Application** - Use cases, command/query handlers, and business rules
  - **Infrastructure** - Data access and external services
  - **Endpoints** - API endpoints and request/response handling

## Features

- **JWT Authentication** - Secure token-based authentication with refresh token support
- **Password Authentication** - Traditional username/password login with secure hashing
- **Role-based Authorization (RBAC)** - Comprehensive role and permission management
- **Command Query Responsibility Segregation (CQRS)** - Separate command and query handlers
- **Repository Pattern** - Clean data access abstraction with Unit of Work
- **Specification Pattern** - Flexible query building and pagination support
- **RESTful API** - FastEndpoints-based high-performance API
- **Swagger Documentation** - Auto-generated API documentation with OpenAPI 3.0
- **Database Migrations** - Entity Framework Core with PostgreSQL support
- **Structured Logging** - Serilog with console and file output, daily log rotation
- **CORS Support** - Configurable cross-origin resource sharing
- **Clean Architecture** - Domain-driven design with clear separation of concerns

## Technology Stack

### Core Framework

- **.NET 9** - Latest .NET framework with improved performance
- **FastEndpoints 7.0.1** - High-performance, minimal API framework
- **C# 12** - Latest language features with nullable reference types

### Data & Persistence

- **Entity Framework Core 9.0.7** - Modern ORM with advanced querying
- **PostgreSQL** - Robust relational database with Npgsql provider
- **Repository Pattern** - Clean data access abstraction
- **Unit of Work Pattern** - Transaction management

### Authentication & Security

- **JWT Bearer Authentication** - Stateless token-based security
- **Microsoft.AspNetCore.Authentication.JwtBearer 9.0.7** - JWT middleware
- **Password Hashing** - Secure credential storage
- **CORS** - Cross-origin resource sharing support

### Architecture Patterns

- **CQRS** - Command Query Responsibility Segregation
- **Specification Pattern** - Flexible query building
- **Clean Architecture** - Domain-driven design principles
- **Modular Monolith** - Organized, maintainable codebase

### Logging & Monitoring

- **Serilog 4.3.0** - Structured logging framework
- **Console & File Sinks** - Multiple output targets
- **Daily Log Rotation** - Automated log management

### Documentation & Development

- **Swagger/OpenAPI 3.0** - Interactive API documentation
- **FastEndpoints.Swagger 7.0.1** - Auto-generated documentation
- **Code Generation** - FastEndpoints.Generator for improved performance

## Getting Started

### Prerequisites

- .NET 9 SDK
- PostgreSQL database
- Visual Studio 2022 or VS Code

### Configuration

1. **Database Configuration**
   Update `appsettings.Development.json` with your PostgreSQL connection details:

   ```json
   {
     "Postgresql": {
       "Host": "your-postgres-host",
       "Port": "5432",
       "Username": "your-username",
       "Password": "your-password",
       "Database": "your-database"
     }
   }
   ```

2. **JWT Configuration**
   Configure JWT settings in `appsettings.json`:
   ```json
   {
     "Jwt": {
       "Secret": "your-super-secret-jwt-key-with-at-least-32-characters",
       "Issuer": "YourApp.Issuer",
       "Audience": "YourApp.Audience",
       "AccessTokenExpirationMinutes": 15,
       "RefreshTokenExpirationDays": 7
     }
   }
   ```

### Installation & Setup

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd MeUi
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Run database migrations**

   ```bash
   dotnet ef database update --project src/MeUi.Entry
   ```

4. **Build the solution**

   ```bash
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run --project src/MeUi.Entry
   ```

The API will be available at `https://localhost:5001` (or the port specified in launchSettings.json).

## API Documentation

Once the application is running, you can access:

- **Swagger UI**: `https://localhost:5001/swagger`
- **OpenAPI Spec**: `https://localhost:5001/swagger/v1/swagger.json`

## Default Credentials

The system includes a default super admin user:

- **Email**: admin@mataelang.com
- **Password**: SuperAdmin123!

## Project Structure

```
src/
├── MeUi.Entry/                     # Main application entry point
│   ├── Extensions/                 # Service registration extensions
│   ├── Middlewares/               # Custom middleware components
│   └── Program.cs                 # Application startup
├── MeUi.Shared/                   # Shared components
│   ├── MeUi.Shared.Application/   # Shared application logic (CQRS, specifications)
│   ├── MeUi.Shared.ApplicationContract/ # Shared contracts and DTOs
│   ├── MeUi.Shared.Domain/        # Shared domain models and entities
│   ├── MeUi.Shared.Endpoint/      # Shared endpoint utilities
│   └── MeUi.Shared.Infrastructure/ # Shared infrastructure (EF, repositories)
├── MeUi.Authentication/           # Authentication module
│   ├── MeUi.Authentication.Core/  # Core authentication infrastructure
│   │   ├── Private/               # Internal implementation
│   │   │   ├── Application/       # Command/query handlers
│   │   │   ├── Domain/           # Authentication domain models
│   │   │   ├── Infrastructure/   # Data access and external services
│   │   │   └── Endpoint/         # Authentication endpoints
│   │   └── Public/               # External contracts and extensions
│   │       ├── ApplicationContract/ # Authentication contracts
│   │       ├── Extension/        # Service registration
│   │       └── Shared/           # Shared authentication components
│   └── MeUi.Authentication.Password/ # Password authentication
│       ├── Private/              # Internal implementation
│       │   ├── Application/      # Password-specific handlers
│       │   ├── Domain/          # Password domain logic
│       │   ├── Infrastructure/  # Password data access
│       │   └── Endpoints/       # Password authentication endpoints
│       └── Public/              # External contracts and extensions
└── MeUi.Authorization/           # Authorization module
    ├── MeUi.Authorization.Core/  # Core authorization infrastructure
    │   ├── Private/              # Internal implementation
    │   └── Public/               # External contracts and extensions
    └── MeUi.Authorization.Rbac/  # Role-based access control
        ├── Private/              # RBAC implementation
        │   ├── Application/      # Role/permission handlers
        │   ├── Domain/          # RBAC domain models
        │   ├── Infrastructure/  # RBAC data access
        │   └── Endpoint/        # RBAC endpoints
        └── Public/              # RBAC contracts and extensions
```

## Development

### Adding New Modules

1. Create the module structure following the established pattern:

   ```
   ModuleName/
   ├── Private/
   │   ├── Application/     # Command/Query handlers
   │   ├── Domain/         # Domain entities and logic
   │   ├── Infrastructure/ # Data access implementation
   │   └── Endpoint/       # API endpoints
   └── Public/
       ├── ApplicationContract/ # DTOs and contracts
       └── Extension/          # Service registration
   ```

2. Add project references to the solution file
3. Create extension methods for service registration
4. Register services in `Program.cs`
5. Add database context and migrations if needed

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/MeUi.Entry

# Update database
dotnet ef database update --project src/MeUi.Entry

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/MeUi.Entry

# Generate SQL script
dotnet ef migrations script --project src/MeUi.Entry
```

### Code Patterns

#### Command/Query Handlers

```csharp
// Command Handler
public class CreateUserCommandHandler : BaseCommandHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken ct)
    {
        // Implementation
    }
}

// Query Handler
public class GetUsersQueryHandler : BaseQueryHandler<GetUsersQuery, GetUsersResponse>
{
    public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken ct)
    {
        // Implementation
    }
}
```

#### Specifications

```csharp
public class UserPaginationSpecification : BasePaginationSpecification<User>
{
    public UserPaginationSpecification(int page, int pageSize, string? searchTerm = null)
        : base(page, pageSize)
    {
        if (!string.IsNullOrEmpty(searchTerm))
        {
            AddCriteria(u => u.Email.Contains(searchTerm) || u.FirstName.Contains(searchTerm));
        }
    }
}
```

### Logging

The application uses Serilog for structured logging:

- **Console output** - Formatted for development readability
- **File logging** - Daily rotation with 7-day retention
- **Structured data** - JSON properties for better querying
- **Configurable levels** - Per-namespace log level control

#### Log Levels Configuration

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "Microsoft.EntityFrameworkCore": "Information"
      }
    }
  }
}
```

### Testing

The project structure supports easy unit testing:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/MeUi.Authentication.Tests/
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For questions and support, please open an issue in the repository or contact the development team.
