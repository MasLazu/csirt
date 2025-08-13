using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventComparativeTimelineAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventComparativeTimelineAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventComparativeTimelineAnalyticsEndpoint : BaseEndpoint<GetTenantThreatEventComparativeTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto>, ITenantPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/timeline/comparative");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant comparative timeline analytics")
            .WithDescription("Returns current vs previous period bucketed counts and trend direction for a tenant."));
    }

    public override async Task HandleAsync(GetTenantThreatEventComparativeTimelineAnalyticsQuery req, CancellationToken ct)
    {
        var tenantId = Route<Guid>("tenantId");
        var enriched = req with { TenantId = tenantId };
        var result = await Mediator.Send(enriched, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Timeline.Count} tenant comparative timeline buckets", ct);
    }
}
