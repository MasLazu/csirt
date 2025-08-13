using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopAsnsAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventTopAsnsAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventTopAsnsAnalyticsEndpoint : BaseEndpoint<GetTenantThreatEventTopAsnsAnalyticsQuery, ThreatEventAsnTopAnalyticsDto>, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/asns/top");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant top ASNs analytics")
            .WithDescription("Returns tenant-scoped top ASNs with counts, percentages, top categories, source IP samples, and average risk score."));
    }

    public override async Task HandleAsync(GetTenantThreatEventTopAsnsAnalyticsQuery req, CancellationToken ct)
    {
        var tenantId = Route<Guid>("tenantId");
        var enriched = req with { TenantId = tenantId };
        var result = await Mediator.Send(enriched, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Asns.Count} tenant ASN entries", ct);
    }
}
