using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatNetwork.Queries.GetMostTargetedPorts;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatNetwork;

public class GetTenantMostTargetedPortsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantMostTargetedPortsQuery, List<TargetedPortDto>, GetTenantMostTargetedPortsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";
    public static string TenantPermission => "READ:TENANT_THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-network/most-targeted-ports");
        Description(x => x.WithTags("Tenant Threat Network")
            .WithSummary("Get tenant most targeted ports")
            .WithDescription("Retrieves most targeted ports bar chart data for tenant-specific network intelligence. Requires READ:TENANT_THREAT_NETWORK permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantMostTargetedPortsQuery req, Guid userId, CancellationToken ct)
    {
        var ports = await Mediator.Send(req, ct);
        await SendSuccessAsync(ports, $"Retrieved {ports.Count} most targeted ports for tenant {req.TenantId}", ct);
    }
}