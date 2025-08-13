using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventGeoHeatmapAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventGeoHeatmapAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventGeoHeatmapAnalyticsEndpoint : BaseEndpoint<GetTenantThreatEventGeoHeatmapAnalyticsQuery, ThreatEventGeoAnalyticsDto>, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/geo/heatmap");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant geo heatmap analytics")
            .WithDescription("Returns tenant-scoped geographic distribution (heatmap) of threat event sources."));
    }

    public override async Task HandleAsync(GetTenantThreatEventGeoHeatmapAnalyticsQuery req, CancellationToken ct)
    {
        var tenantId = Route<Guid>("tenantId");
        var enriched = req with { TenantId = tenantId };
        var result = await Mediator.Send(enriched, ct);
        await SendSuccessAsync(result, $"Retrieved tenant geo heatmap with {result.SourceCountries.Count} countries", ct);
    }
}
