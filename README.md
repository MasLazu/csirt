# MeUi API

A modern .NET 9 Web API built with Clean Architecture principles, featuring JWT authentication, PostgreSQL database integration, and FastEndpoints for high-performance API development.

## Architecture

This project follows Clean Architecture principles with clear separation of concerns:

- **MeUi.Api** - Web API layer with endpoints and middleware
- **MeUi.Application** - Business logic and application services
- **MeUi.Domain** - Core domain entities and business rules
- **MeUi.Infrastructure** - Data access, external services, and infrastructure concerns

## Tech Stack

- **.NET 9** - Latest .NET framework
- **FastEndpoints** - High-performance alternative to MVC controllers
- **MediatR** - CQRS and mediator pattern implementation
- **Entity Framework Core** - ORM for database operations
- **PostgreSQL** - Primary database
- **JWT Authentication** - Secure token-based authentication
- **BCrypt** - Password hashing
- **Serilog** - Structured logging
- **FluentValidation** - Input validation
- **Mapster** - Object mapping
- **Swagger/OpenAPI** - API documentation

## Features

- **Authentication & Authorization**

  - JWT token-based authentication
  - Refresh token support
  - Secure password hashing with BCrypt
  - Cookie-based refresh token storage

- **API Design**

  - RESTful API endpoints
  - FastEndpoints for optimal performance
  - Comprehensive error handling
  - CORS support for frontend integration

- **Data Management**

  - PostgreSQL database integration
  - Entity Framework Core with migrations
  - Database seeding for initial data

- **Logging & Monitoring**
  - Structured logging with Serilog
  - File and console logging
  - Configurable log levels

## Getting Started

### Prerequisites

- .NET 9 SDK
- PostgreSQL database
- Visual Studio 2022 or VS Code

### Configuration

1. Update the database connection string in `appsettings.json`:

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

2. Configure JWT settings:

```json
{
  "Jwt": {
    "Key": "your-super-secret-jwt-key-that-is-at-least-32-characters-long",
    "Issuer": "MeUi.UnifiedApi",
    "Audience": "MeUi.Users",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

3. Set up CORS origins for your frontend:

```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "https://localhost:3001"]
  }
}
```

### Running the Application

1. **Restore dependencies:**

   ```bash
   dotnet restore
   ```

2. **Run database migrations:**

   ```bash
   dotnet ef database update --project src/MeUi.Infrastructure --startup-project src/MeUi.Api
   ```

3. **Start the application:**

   ```bash
   dotnet run --project src/MeUi.Api
   ```

4. **Access the API:**
   - API: `https://localhost:7239` or `http://localhost:5000`
   - Swagger UI: `https://localhost:7239/swagger`

## API Endpoints

### Authentication

- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refresh` - Refresh access token
- `POST /api/v1/auth/logout` - User logout

### Users

- User management endpoints (implementation varies)

## Project Structure

```
src/
├── MeUi.Api/                 # Web API layer
│   ├── Endpoints/            # FastEndpoints definitions
│   ├── Middlewares/          # Custom middleware
│   ├── Models/               # API models and responses
│   └── Program.cs            # Application entry point
├── MeUi.Application/         # Application layer
│   ├── Features/             # Feature-based organization
│   ├── Interfaces/           # Application interfaces
│   └── Models/               # Application models
├── MeUi.Domain/              # Domain layer
│   └── Entities/             # Domain entities
└── MeUi.Infrastructure/      # Infrastructure layer
    ├── Data/                 # Database context and configurations
    └── Services/             # Infrastructure services
```

## Development

### Adding New Features

1. **Domain Layer**: Define entities in `MeUi.Domain/Entities/`
2. **Application Layer**: Create commands/queries in `MeUi.Application/Features/`
3. **Infrastructure Layer**: Implement repositories and services
4. **API Layer**: Create endpoints in `MeUi.Api/Endpoints/`

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/MeUi.Infrastructure --startup-project src/MeUi.Api

# Update database
dotnet ef database update --project src/MeUi.Infrastructure --startup-project src/MeUi.Api
```

## Security

- Passwords are hashed using BCrypt
- JWT tokens for stateless authentication
- Refresh tokens stored as HTTP-only cookies
- CORS configured for specific origins
- Global exception handling to prevent information leakage

## Logging

Logs are written to both console and files:

- Console: Formatted for development
- Files: Located in `logs/` directory with daily rotation
- Configurable log levels via `appsettings.json`

> Threat analytics implementation progress has been moved to `THREAT_ANALYTICS_ROADMAP.md` for clarity.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.
