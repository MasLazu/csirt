using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatNetwork.Queries.GetProtocolTrends;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatNetwork;

public class GetTenantProtocolTrendsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantProtocolTrendsQuery, List<ProtocolTrendDto>, GetTenantProtocolTrendsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";
    public static string TenantPermission => "READ:TENANT_THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-network/protocol-trends");
        Description(x => x.WithTags("Tenant Threat Network")
            .WithSummary("Get tenant protocol usage trends")
            .WithDescription("Retrieves protocol usage trends time series data for tenant-specific network intelligence. Requires READ:TENANT_THREAT_NETWORK permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantProtocolTrendsQuery req, Guid userId, CancellationToken ct)
    {
        var trends = await Mediator.Send(req, ct);
        await SendSuccessAsync(trends, $"Retrieved {trends.Count} protocol trend data points for tenant {req.TenantId}", ct);
    }
}