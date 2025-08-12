using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using FluentValidation;
using MeUi.Api.Endpoints.Tenants;
using MeUi.Api.Middlewares;
using MeUi.Application;
using MeUi.Application.Features.Tenants.Commands.CreateTenant;
using MeUi.Infrastructure;
using MeUi.Infrastructure.Data.Seeders;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

try
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    builder.Host.UseSerilog();

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration)
        .AddFastEndpoints()
        .AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Jwt:Key"] ?? "The secret used to sign tokens")
        .AddAuthorization()
        .SwaggerDocument();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
        {
            string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }));

    Log.Information("Starting MeUi API application");

    WebApplication app = builder.Build();

    app.UseExceptionHandler()
       .UseCors()
       .UseAuthentication()
       .UseAuthorization()
       .UseFastEndpoints()
       .UseSwaggerGen();

    {
        using IServiceScope scope = app.Services.CreateScope();
        DatabaseSeeder seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }

    Log.Information("MeUi API application configured successfully");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}