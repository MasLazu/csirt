using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatGeographic.Queries.GetCrossBorderAttackFlows;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatGeographic;

public class GetTenantCrossBorderAttackFlowsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantCrossBorderAttackFlowsQuery, List<CrossBorderAttackFlowDto>, GetTenantCrossBorderAttackFlowsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";
    public static string TenantPermission => "READ:TENANT_THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-geographic/cross-border-flows");
        Description(x => x.WithTags("Tenant Threat Geographic")
            .WithSummary("Get tenant cross-border attack flows")
            .WithDescription("Retrieves analysis of attack flows between source and destination countries for tenant-specific threat intelligence. Requires READ:TENANT_THREAT_GEOGRAPHIC permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantCrossBorderAttackFlowsQuery req, Guid userId, CancellationToken ct)
    {
        var flows = await Mediator.Send(req, ct);
        await SendSuccessAsync(flows, $"Retrieved {flows.Count} cross-border attack flows for tenant {req.TenantId}", ct);
    }
}