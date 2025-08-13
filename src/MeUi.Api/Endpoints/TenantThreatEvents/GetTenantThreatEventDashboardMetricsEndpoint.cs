using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventDashboardMetrics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventDashboardMetrics;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventDashboardMetricsEndpoint : BaseEndpoint<GetTenantThreatEventDashboardMetricsQuery, ThreatEventDashboardMetricsDto>, ITenantPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/dashboard");
        Description(d => d.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant dashboard metrics")
            .WithDescription("Returns high-level dashboard metrics for a specific tenant (last 24h & hour)."));
    }

    public override async Task HandleAsync(GetTenantThreatEventDashboardMetricsQuery req, CancellationToken ct)
    {
        var tenantId = Route<Guid>("tenantId");
        if (tenantId == Guid.Empty)
        {
            AddError("tenantId", "Invalid tenant id.");
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        req = new GetTenantThreatEventDashboardMetricsQuery { TenantId = tenantId };
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Retrieved tenant dashboard metrics", ct);
    }
}
