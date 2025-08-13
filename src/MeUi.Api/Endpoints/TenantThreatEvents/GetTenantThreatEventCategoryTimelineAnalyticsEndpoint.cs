using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventCategoryTimelineAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventCategoryTimelineAnalyticsEndpoint : BaseEndpoint<GetTenantThreatEventCategoryTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto>, ITenantPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/categories/timeline");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant category timeline analytics")
            .WithDescription("Returns per-bucket event counts filtered (optionally) to a specific category with category distribution per bucket for a tenant."));
    }

    public override async Task HandleAsync(GetTenantThreatEventCategoryTimelineAnalyticsQuery req, CancellationToken ct)
    {
        var tenantId = Route<Guid>("tenantId");
        var enriched = req with { TenantId = tenantId };
        var result = await Mediator.Send(enriched, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Timeline.Count} tenant category timeline buckets", ct);
    }
}
