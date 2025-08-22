using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatCompliance.Queries.GetAttackCategoryAnalysis;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatCompliance;

public class GetTenantAttackCategoryAnalysisEndpoint : BaseTenantAuthorizedEndpoint<GetTenantAttackCategoryAnalysisQuery, List<AttackCategoryDto>, GetTenantAttackCategoryAnalysisEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";
    public static string TenantPermission => "READ:TENANT_THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-compliance/attack-category-analysis");
        Description(x => x.WithTags("Tenant Threat Compliance")
            .WithSummary("Get tenant attack category analysis")
            .WithDescription("Retrieves attack category analysis for tenant-specific threat compliance monitoring. Requires READ:TENANT_THREAT_COMPLIANCE permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantAttackCategoryAnalysisQuery req, Guid userId, CancellationToken ct)
    {
        var analysis = await Mediator.Send(req, ct);
        await SendSuccessAsync(analysis, $"Retrieved {analysis.Count} attack category analysis entries for tenant {req.TenantId}", ct);
    }
}