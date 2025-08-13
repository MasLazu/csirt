using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTimelineAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventTimelineAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventTimelineAnalyticsEndpoint : BaseEndpoint<GetTenantThreatEventTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto>, ITenantPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/timeline");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant threat event timeline analytics")
            .WithDescription("Retrieves tenant-scoped time-series analytics with configurable intervals and filtering."));
    }

    public override async Task HandleAsync(GetTenantThreatEventTimelineAnalyticsQuery req, CancellationToken ct)
    {
        var tenantId = Route<Guid>("tenantId");
        var enriched = req with { TenantId = tenantId };
        var analytics = await Mediator.Send(enriched, ct);
        await SendSuccessAsync(analytics, $"Retrieved tenant timeline analytics with {analytics.Timeline.Count} data points", ct);
    }
}
