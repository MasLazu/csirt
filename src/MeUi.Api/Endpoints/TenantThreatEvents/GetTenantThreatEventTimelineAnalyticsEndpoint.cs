using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTimelineAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventTimelineAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventTimelineAnalyticsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatEventTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto, GetTenantThreatEventTimelineAnalyticsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:TENANT_THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/timeline");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant threat event timeline analytics")
            .WithDescription("Retrieves tenant-scoped time-series analytics with configurable intervals and filtering."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatEventTimelineAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventTimelineAnalyticsDto analytics = await Mediator.Send(req, ct);
        await SendSuccessAsync(analytics, $"Retrieved tenant timeline analytics with {analytics.Timeline.Count} data points", ct);
    }
}
