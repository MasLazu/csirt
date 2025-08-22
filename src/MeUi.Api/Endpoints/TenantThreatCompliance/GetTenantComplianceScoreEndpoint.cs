using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatCompliance.Queries.GetComplianceScore;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantThreatCompliance;

public class GetTenantComplianceScoreEndpoint : BaseTenantAuthorizedEndpoint<GetTenantComplianceScoreQuery, ComplianceScoreDto, GetTenantComplianceScoreEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";
    public static string TenantPermission => "READ:TENANT_THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-compliance/compliance-score");
        Description(x => x.WithTags("Tenant Threat Compliance")
            .WithSummary("Get tenant compliance score")
            .WithDescription("Retrieves current compliance score for tenant-specific threat monitoring. Requires READ:TENANT_THREAT_COMPLIANCE permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantComplianceScoreQuery req, Guid userId, CancellationToken ct)
    {
        var score = await Mediator.Send(req, ct);
        await SendSuccessAsync(score, $"Retrieved compliance score {score.ComplianceScore} for tenant {req.TenantId}", ct);
    }
}