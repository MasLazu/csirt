using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MeUi.Application.Features.ThreatIntelligence.Interfaces;
using MeUi.Application.Features.ThreatIntelligence.Services;

namespace MeUi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // Register threat intelligence services
        services.AddScoped<IThreatIntelligenceQueryService, ThreatIntelligenceQueryService>();

        return services;
    }
}