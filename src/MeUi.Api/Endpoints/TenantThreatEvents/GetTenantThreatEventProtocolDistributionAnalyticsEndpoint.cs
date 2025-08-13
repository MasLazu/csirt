using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventProtocolDistributionAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventProtocolDistributionAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventProtocolDistributionAnalyticsEndpoint : BaseEndpoint<GetTenantThreatEventProtocolDistributionAnalyticsQuery, ThreatEventProtocolDistributionAnalyticsDto>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/protocols/distribution");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant protocol distribution analytics")
            .WithDescription("Returns tenant-scoped protocol distribution including top ports & categories."));
    }

    public override async Task HandleAsync(GetTenantThreatEventProtocolDistributionAnalyticsQuery req, CancellationToken ct)
    {
        var tenantId = Route<Guid>("tenantId");
        var enriched = req with { TenantId = tenantId };
        var result = await Mediator.Send(enriched, ct);
        await SendSuccessAsync(result, $"Retrieved tenant protocol distribution with {result.Protocols.Count} protocols", ct);
    }
}
