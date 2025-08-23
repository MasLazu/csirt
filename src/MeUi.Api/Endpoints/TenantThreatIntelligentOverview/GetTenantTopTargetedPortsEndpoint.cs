using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetTopTargetedPorts;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIntelligentOverview;

public class GetTenantTopTargetedPortsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantTopTargetedPortsQuery, List<TargetedPortDto>, GetTenantTopTargetedPortsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:TENANT_THREAT_INTELLIGENT_OVERVIEW";
    public static string TenantPermission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-intelligent-overview/top-targeted-ports");
        Description(x => x.WithTags("Tenant Threat Intelligent Overview")
            .WithSummary("Get tenant top targeted ports")
            .WithDescription("Retrieves top targeted ports for tenant-specific threat intelligence overview. Requires READ:TENANT_THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantTopTargetedPortsQuery req, Guid userId, CancellationToken ct)
    {
        List<TargetedPortDto> ports = await Mediator.Send(req, ct);
        await SendSuccessAsync(ports, $"Retrieved {ports.Count} top targeted ports for tenant {req.TenantId}", ct);
    }
}