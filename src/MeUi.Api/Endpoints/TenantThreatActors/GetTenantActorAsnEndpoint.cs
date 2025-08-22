using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatActors.Queries.GetActorAsn;
using MeUi.Application.Models.ThreatActors;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatActors;

public class GetTenantActorAsnEndpoint : BaseTenantAuthorizedEndpoint<GetTenantActorAsnQuery, List<ActorAsnDto>, GetTenantActorAsnEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";
    public static string TenantPermission => "READ:TENANT_THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-actors/asn");
        Description(x => x.WithTags("Tenant Threat Actors")
            .WithSummary("Get tenant actor ASN analysis")
            .WithDescription("Retrieves threat actor ASN concentration analysis for tenant-specific data. Requires READ:TENANT_THREAT_ACTORS permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantActorAsnQuery req, Guid userId, CancellationToken ct)
    {
        var asns = await Mediator.Send(req, ct);
        await SendSuccessAsync(asns, $"Retrieved {asns.Count} ASN distributions for tenant {req.TenantId}", ct);
    }
}