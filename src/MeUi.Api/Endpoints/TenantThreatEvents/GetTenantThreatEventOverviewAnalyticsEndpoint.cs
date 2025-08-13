using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventOverviewAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventOverviewAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventOverviewAnalyticsEndpoint : BaseEndpoint<GetTenantThreatEventOverviewAnalyticsQuery, ThreatEventOverviewAnalyticsDto>, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS"; // tenant-scoped grant
    public static string Permission => "READ:THREAT_ANALYTICS";       // global override

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/overview");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant threat event overview analytics")
            .WithDescription("Retrieves tenant-scoped lightweight overview metrics including core counts and top entities."));
    }

    public override async Task HandleAsync(GetTenantThreatEventOverviewAnalyticsQuery req, CancellationToken ct)
    {
        var tenantId = Route<Guid>("tenantId");
        var enriched = req with { TenantId = tenantId };
        var analytics = await Mediator.Send(enriched, ct);
        await SendSuccessAsync(analytics, $"Retrieved tenant overview with {analytics.TopCategories.Count} categories", ct);
    }
}
