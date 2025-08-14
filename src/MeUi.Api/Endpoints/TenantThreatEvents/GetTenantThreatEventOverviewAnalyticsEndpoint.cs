using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventOverviewAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventOverviewAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventOverviewAnalyticsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatEventOverviewAnalyticsQuery, ThreatEventOverviewAnalyticsDto, GetTenantThreatEventOverviewAnalyticsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:TENANT_THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/overview");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant threat event overview analytics")
            .WithDescription("Retrieves tenant-scoped lightweight overview metrics including core counts and top entities."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatEventOverviewAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventOverviewAnalyticsDto analytics = await Mediator.Send(req, ct);
        await SendSuccessAsync(analytics, $"Retrieved tenant overview with {analytics.TopCategories.Count} categories", ct);
    }
}
