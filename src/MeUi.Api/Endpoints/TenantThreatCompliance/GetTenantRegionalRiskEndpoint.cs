using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatCompliance.Queries.GetRegionalRisk;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatCompliance;

public class GetTenantRegionalRiskEndpoint : BaseTenantAuthorizedEndpoint<GetTenantRegionalRiskQuery, List<RegionalRiskDto>, GetTenantRegionalRiskEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";
    public static string TenantPermission => "READ:TENANT_THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-compliance/regional-risk");
        Description(x => x.WithTags("Tenant Threat Compliance")
            .WithSummary("Get tenant regional risk analysis")
            .WithDescription("Retrieves regional risk assessment for tenant-specific threat compliance monitoring. Requires READ:TENANT_THREAT_COMPLIANCE permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantRegionalRiskQuery req, Guid userId, CancellationToken ct)
    {
        var regionalRisk = await Mediator.Send(req, ct);
        await SendSuccessAsync(regionalRisk, $"Retrieved {regionalRisk.Count} regional risk entries for tenant {req.TenantId}", ct);
    }
}