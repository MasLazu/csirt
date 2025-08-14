using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventDashboardMetrics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventDashboardMetrics;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventDashboardMetricsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatEventDashboardMetricsQuery, ThreatEventDashboardMetricsDto, GetTenantThreatEventDashboardMetricsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:TENANT_THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/dashboard");
        Description(d => d.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant dashboard metrics")
            .WithDescription("Returns high-level dashboard metrics for a specific tenant (last 24h & hour)."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatEventDashboardMetricsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventDashboardMetricsDto result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Retrieved tenant dashboard metrics", ct);
    }
}
