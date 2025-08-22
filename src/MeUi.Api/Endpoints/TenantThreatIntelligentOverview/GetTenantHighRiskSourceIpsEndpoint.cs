using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetHighRiskSourceIps;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatIntelligentOverview;

public class GetTenantHighRiskSourceIpsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantHighRiskSourceIpsQuery, List<HighRiskSourceIpDto>, GetTenantHighRiskSourceIpsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";
    public static string TenantPermission => "READ:TENANT_THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-intelligent-overview/high-risk-source-ips");
        Description(x => x.WithTags("Tenant Threat Intelligent Overview")
            .WithSummary("Get tenant high-risk source IPs")
            .WithDescription("Retrieves high-risk source IPs for tenant-specific threat intelligence overview. Requires READ:TENANT_THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantHighRiskSourceIpsQuery req, Guid userId, CancellationToken ct)
    {
        var ips = await Mediator.Send(req, ct);
        await SendSuccessAsync(ips, $"Retrieved {ips.Count} high-risk source IPs for tenant {req.TenantId}", ct);
    }
}