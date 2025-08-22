using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatCompliance.Queries.GetCurrentRiskLevel;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantThreatCompliance;

public class GetTenantCurrentRiskLevelEndpoint : BaseTenantAuthorizedEndpoint<GetTenantCurrentRiskLevelQuery, RiskLevelDto, GetTenantCurrentRiskLevelEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";
    public static string TenantPermission => "READ:TENANT_THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-compliance/current-risk-level");
        Description(x => x.WithTags("Tenant Threat Compliance")
            .WithSummary("Get tenant current risk level")
            .WithDescription("Retrieves current risk level assessment for tenant-specific threat monitoring. Requires READ:TENANT_THREAT_COMPLIANCE permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantCurrentRiskLevelQuery req, Guid userId, CancellationToken ct)
    {
        var riskLevel = await Mediator.Send(req, ct);
        await SendSuccessAsync(riskLevel, $"Retrieved risk level {riskLevel.RiskLevel} for tenant {req.TenantId}", ct);
    }
}