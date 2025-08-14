using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventCategoryTimelineAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventCategoryTimelineAnalyticsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatEventCategoryTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto, GetTenantThreatEventCategoryTimelineAnalyticsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:TENANT_THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/categories/timeline");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant category timeline analytics")
            .WithDescription("Returns per-bucket event counts filtered (optionally) to a specific category with category distribution per bucket for a tenant."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatEventCategoryTimelineAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventTimelineAnalyticsDto result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Timeline.Count} tenant category timeline buckets", ct);
    }
}
