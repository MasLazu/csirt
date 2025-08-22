using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatNetwork.Queries.GetAsnNetworkAnalysis;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatNetwork;

public class GetTenantAsnNetworkAnalysisEndpoint : BaseTenantAuthorizedEndpoint<GetTenantAsnNetworkAnalysisQuery, List<AsnNetworkDto>, GetTenantAsnNetworkAnalysisEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";
    public static string TenantPermission => "READ:TENANT_THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-network/asn-network-analysis");
        Description(x => x.WithTags("Tenant Threat Network")
            .WithSummary("Get tenant ASN network analysis")
            .WithDescription("Retrieves ASN network analysis table data for tenant-specific network intelligence. Requires READ:TENANT_THREAT_NETWORK permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantAsnNetworkAnalysisQuery req, Guid userId, CancellationToken ct)
    {
        var analysis = await Mediator.Send(req, ct);
        await SendSuccessAsync(analysis, $"Retrieved {analysis.Count} ASN network analysis entries for tenant {req.TenantId}", ct);
    }
}