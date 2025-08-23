using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetProtocolDistribution;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIntelligentOverview;

public class GetTenantProtocolDistributionEndpoint : BaseTenantAuthorizedEndpoint<GetTenantProtocolDistributionQuery, List<ProtocolDistributionDto>, GetTenantProtocolDistributionEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:TENANT_THREAT_INTELLIGENT_OVERVIEW";
    public static string TenantPermission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-intelligent-overview/protocol-distribution");
        Description(x => x.WithTags("Tenant Threat Intelligent Overview")
            .WithSummary("Get tenant protocol distribution")
            .WithDescription("Retrieves protocol distribution for tenant-specific threat intelligence overview. Requires READ:TENANT_THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantProtocolDistributionQuery req, Guid userId, CancellationToken ct)
    {
        List<ProtocolDistributionDto> protocols = await Mediator.Send(req, ct);
        await SendSuccessAsync(protocols, $"Retrieved {protocols.Count} protocol distribution entries for tenant {req.TenantId}", ct);
    }
}