using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatNetwork.Queries.GetMostTargetedInfrastructure;
using MeUi.Application.Models.ThreatNetwork;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatNetwork;

public class GetTenantMostTargetedInfrastructureEndpoint : BaseTenantAuthorizedEndpoint<GetTenantMostTargetedInfrastructureQuery, List<TargetedInfrastructureDto>, GetTenantMostTargetedInfrastructureEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_NETWORK";
    public static string TenantPermission => "READ:TENANT_THREAT_NETWORK";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-network/most-targeted-infrastructure");
        Description(x => x.WithTags("Tenant Threat Network")
            .WithSummary("Get tenant most targeted infrastructure")
            .WithDescription("Retrieves most targeted infrastructure table data for tenant-specific network intelligence. Requires READ:TENANT_THREAT_NETWORK permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantMostTargetedInfrastructureQuery req, Guid userId, CancellationToken ct)
    {
        var infrastructure = await Mediator.Send(req, ct);
        await SendSuccessAsync(infrastructure, $"Retrieved {infrastructure.Count} most targeted infrastructure entries for tenant {req.TenantId}", ct);
    }
}