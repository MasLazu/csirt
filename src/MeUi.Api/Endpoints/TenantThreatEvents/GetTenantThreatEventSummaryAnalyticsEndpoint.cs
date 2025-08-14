using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventSummaryAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventSummaryAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventSummaryAnalyticsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatEventSummaryAnalyticsQuery, ThreatEventSummaryAnalyticsDto, GetTenantThreatEventSummaryAnalyticsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:TENANT_THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/summary");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant threat event summary analytics")
            .WithDescription("Retrieves tenant-scoped comprehensive threat intelligence summary including trends and top threats."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatEventSummaryAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventSummaryAnalyticsDto analytics = await Mediator.Send(req, ct);
        await SendSuccessAsync(analytics, $"Retrieved tenant summary with {analytics.ThreatCategories.Count} categories", ct);
    }
}
