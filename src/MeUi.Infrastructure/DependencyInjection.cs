using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MeUi.Application.Interfaces;
using MeUi.Infrastructure.Data;
using MeUi.Infrastructure.Data.Repositories;
using MeUi.Infrastructure.Services;
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