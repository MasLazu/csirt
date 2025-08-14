using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventComparativeTimelineAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventComparativeTimelineAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventComparativeTimelineAnalyticsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatEventComparativeTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto, GetTenantThreatEventComparativeTimelineAnalyticsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:TENANT_THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/timeline/comparative");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant comparative timeline analytics")
            .WithDescription("Returns current vs previous period bucketed counts and trend direction for a tenant."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatEventComparativeTimelineAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventTimelineAnalyticsDto result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Timeline.Count} tenant comparative timeline buckets", ct);
    }
}
