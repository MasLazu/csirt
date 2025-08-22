using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatNetwork.Queries.GetHighRiskIpReputation;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatNetwork;

public class GetTenantHighRiskIpReputationEndpoint : BaseTenantAuthorizedEndpoint<GetTenantHighRiskIpReputationQuery, List<HighRiskIpDto>, GetTenantHighRiskIpReputationEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";
    public static string TenantPermission => "READ:TENANT_THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-network/high-risk-ip-reputation");
        Description(x => x.WithTags("Tenant Threat Network")
            .WithSummary("Get tenant high-risk IP reputation analysis")
            .WithDescription("Retrieves high-risk IP reputation analysis table data for tenant-specific network intelligence. Requires READ:TENANT_THREAT_NETWORK permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantHighRiskIpReputationQuery req, Guid userId, CancellationToken ct)
    {
        var ips = await Mediator.Send(req, ct);
        await SendSuccessAsync(ips, $"Retrieved {ips.Count} high-risk IP addresses for tenant {req.TenantId}", ct);
    }
}