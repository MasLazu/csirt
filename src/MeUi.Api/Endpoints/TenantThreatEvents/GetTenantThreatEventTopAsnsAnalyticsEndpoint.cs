using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopAsnsAnalytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventTopAsnsAnalytics;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Api.Endpoints.TenantThreatEvents;

public class GetTenantThreatEventTopAsnsAnalyticsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantThreatEventTopAsnsAnalyticsQuery, ThreatEventAsnTopAnalyticsDto, GetTenantThreatEventTopAsnsAnalyticsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:THREAT_ANALYTICS";
    public static string Permission => "READ:TENANT_THREAT_ANALYTICS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-events/analytics/asns/top");
        Description(x => x.WithTags("Tenant Threat Event Analytics")
            .WithSummary("Get tenant top ASNs analytics")
            .WithDescription("Returns tenant-scoped top ASNs with counts, percentages, top categories, source IP samples, and average risk score."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantThreatEventTopAsnsAnalyticsQuery req, Guid userId, CancellationToken ct)
    {
        ThreatEventAsnTopAnalyticsDto result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Asns.Count} tenant ASN entries", ct);
    }
}
