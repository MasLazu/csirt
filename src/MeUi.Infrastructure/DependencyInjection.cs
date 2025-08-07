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
            options.UseNpgsql(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<Data.Seeders.DatabaseSeeder>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }

    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure MongoDB BSON serialization
        MongoDbConfiguration.Configure();

        // Register MongoDB client and database
        services.AddSingleton<IMongoClient>(provider =>
        {
            var connectionString = configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
            return new MongoClient(connectionString);
        });

        services.AddScoped<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var databaseName = configuration["MongoDB:DatabaseName"] ?? "meui_threat_db";
            var database = client.GetDatabase(databaseName);

            // Configure indexes asynchronously (fire and forget for startup performance)
            _ = Task.Run(async () =>
            {
                try
                {
                    await MongoDbConfiguration.ConfigureIndexesAsync(database);
                }
                catch (Exception ex)
                {
                    // Log the exception but don't fail the application startup
                    Console.WriteLine($"Warning: Failed to create MongoDB indexes: {ex.Message}");
                }
            });

            return database;
        });

        // Register MongoDB repositories for entities that need MongoDB
        services.AddScoped<IThreatIntelligenceRepository, ThreatIntelligenceRepository>();

        // Register generic MongoDB repository for ThreatIntelligence entity
        services.AddScoped<IRepository<ThreatIntelligence>>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new MongoRepository<ThreatIntelligence>(database);
        });

        // Register MongoDB seeder
        services.AddScoped<Data.Seeders.MongoDbSeeder>();

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