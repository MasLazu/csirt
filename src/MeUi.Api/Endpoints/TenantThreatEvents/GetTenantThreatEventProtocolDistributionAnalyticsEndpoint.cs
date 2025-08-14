using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventProtocolDistributionAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventProtocolDistributionAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventProtocolDistributionAnalyticsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatEventProtocolDistributionAnalyticsQuery, ThreatEventProtocolDistributionAnalyticsDto, GetTenantThreatEventProtocolDistributionAnalyticsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:TENANT_THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/protocols/distribution");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant protocol distribution analytics")
            .WithDescription("Returns tenant-scoped protocol distribution including top ports & categories."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatEventProtocolDistributionAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventProtocolDistributionAnalyticsDto result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved tenant protocol distribution with {result.Protocols.Count} protocols", ct);
    }
}
