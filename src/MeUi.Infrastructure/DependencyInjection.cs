using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Infrastructure.Data;
using MeUi.Infrastructure.Data.Repositories;
using MeUi.Infrastructure.Data.Configurations;
using MeUi.Infrastructure.Services;
using MongoDB.Driver;
using Npgsql;

namespace MeUi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = BuildConnectionString(configuration);
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                // Enable retry on failure for better resilience with TimescaleDB
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);

                // Configure command timeout for long-running TimescaleDB queries
                npgsqlOptions.CommandTimeout(30);
            }));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<Data.Seeders.DatabaseSeeder>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }

    private static string BuildConnectionString(IConfiguration configuration)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = configuration["Postgresql:Host"] ?? "localhost",
            Port = int.Parse(configuration["Postgresql:Port"] ?? "5432"),
            Username = configuration["Postgresql:Username"] ?? "postgres",
            Password = configuration["Postgresql:Password"] ?? "password",
            Database = configuration["Postgresql:Database"] ?? "meui_unified_db"
        };

        return builder.ConnectionString;
    }
}