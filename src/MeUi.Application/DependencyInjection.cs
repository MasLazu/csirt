using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
// Removed ThreatIntelligence interfaces and services - using generic repository pattern
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatTimeSeries;

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

        // ThreatIntelligence services removed - using generic repository pattern

        // Explicitly register the GetThreatTimeSeries handler to ensure it's discovered
        services.AddScoped<IRequestHandler<GetThreatTimeSeriesNewQuery, ThreatTimeSeriesDto>, GetThreatTimeSeriesQueryHandler>();

        return services;
    }
}