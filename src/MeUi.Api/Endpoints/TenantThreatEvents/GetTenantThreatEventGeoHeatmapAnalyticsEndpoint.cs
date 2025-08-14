using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventGeoHeatmapAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventGeoHeatmapAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventGeoHeatmapAnalyticsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatEventGeoHeatmapAnalyticsQuery, ThreatEventGeoAnalyticsDto, GetTenantThreatEventGeoHeatmapAnalyticsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:TENANT_THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/geo/heatmap");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant geo heatmap analytics")
            .WithDescription("Returns tenant-scoped geographic distribution (heatmap) of threat event sources."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatEventGeoHeatmapAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventGeoAnalyticsDto result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved tenant geo heatmap with {result.SourceCountries.Count} countries", ct);
    }
}
