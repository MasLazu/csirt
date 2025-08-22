using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatNetwork.Queries.GetProtocolDistribution;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatNetwork;

public class GetTenantProtocolDistributionEndpoint : BaseTenantAuthorizedEndpoint<GetTenantProtocolDistributionQuery, List<ProtocolDistributionDto>, GetTenantProtocolDistributionEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";
    public static string TenantPermission => "READ:TENANT_THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-network/protocol-distribution");
        Description(x => x.WithTags("Tenant Threat Network")
            .WithSummary("Get tenant protocol distribution")
            .WithDescription("Retrieves protocol usage distribution pie chart data for tenant-specific network intelligence. Requires READ:TENANT_THREAT_NETWORK permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantProtocolDistributionQuery req, Guid userId, CancellationToken ct)
    {
        var distribution = await Mediator.Send(req, ct);
        await SendSuccessAsync(distribution, $"Retrieved protocol distribution with {distribution.Count} protocols for tenant {req.TenantId}", ct);
    }
}