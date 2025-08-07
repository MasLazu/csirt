# Database Migrations and Seeding

This document explains how to work with database migrations and seeding in the MeUi unified application.

## Overview

The application uses Entity Framework Core migrations to manage database schema changes and includes an automatic seeding system for initial data.

## Migration Commands

All migration commands should be run from the solution root directory.

### Create a New Migration

```bash
dotnet ef migrations add <MigrationName> --project src-refactor/MeUi.Infrastructure --startup-project src-refactor/MeUi.Api --output-dir Data/Migrations
```

### Apply Migrations to Database

```bash
dotnet ef database update --project src-refactor/MeUi.Infrastructure --startup-project src-refactor/MeUi.Api
```

### List All Migrations

```bash
dotnet ef migrations list --project src-refactor/MeUi.Infrastructure --startup-project src-refactor/MeUi.Api
```

### Remove Last Migration (if not applied to database)

```bash
dotnet ef migrations remove --project src-refactor/MeUi.Infrastructure --startup-project src-refactor/MeUi.Api
```

## Database Schema

The unified database includes the following main tables:

### Authentication Tables

- **Users** - User accounts
- **LoginMethods** - Available authentication methods (Password, OAuth, etc.)
- **UserLoginMethods** - Links users to their authentication methods
- **Passwords** - Password hashes for password-based authentication
- **RefreshTokens** - JWT refresh tokens

### Authorization Tables

- **Roles** - User roles (SuperAdmin, Admin, User, Guest)
- **UserRoles** - Links users to their roles
- **Resources** - System resources that can be protected
- **Actions** - Actions that can be performed on resources
- **Permissions** - Combinations of actions and resources
- **RolePermissions** - Links roles to their permissions

## Automatic Database Initialization

The application automatically:

1. **Applies pending migrations** on startup
2. **Seeds initial data** including:
   - Login methods (Password, OAuth, Two Factor)
   - Default roles (SuperAdmin, Admin, User, Guest)
   - System resources and actions (discovered from code)
   - Super user account (from configuration)

## Configuration

Database connection is configured in `appsettings.json`:

```json
{
  "Postgresql": {
    "Host": "postgresql.mfaziz.cloud",
    "Port": "5432",
    "Username": "postgres",
    "Password": "FazizHomelab4212",
    "Database": "csrit"
  },
  "SuperUser": {
    "Email": "admin@meui.com",
    "Username": "superadmin",
    "Password": "SuperAdmin123!",
    "FirstName": "Super",
    "LastName": "Admin"
  }
}
```

## Development Workflow

1. **Make entity changes** in the Domain layer
2. **Create migration** using the command above
3. **Review generated migration** for correctness
4. **Apply migration** to your development database
5. **Test the changes** thoroughly
6. **Commit migration files** to version control

## Production Deployment

The application will automatically apply migrations and seed data on startup. Ensure:

1. Database credentials are properly configured
2. Database server is accessible
3. Application has necessary permissions to create/modify database objects

## Troubleshooting

### Migration Fails

- Check database connectivity
- Verify credentials in configuration
- Ensure database exists
- Check for conflicting schema changes

### Seeding Fails

- Check application logs for specific errors
- Verify configuration values (especially SuperUser settings)
- Ensure required reference data is not missing

### PostgreSQL Syntax Issues

- Ensure migration uses PostgreSQL-compatible syntax
- Check for SQL Server specific syntax (brackets instead of quotes)
- Verify data types are PostgreSQL compatible
