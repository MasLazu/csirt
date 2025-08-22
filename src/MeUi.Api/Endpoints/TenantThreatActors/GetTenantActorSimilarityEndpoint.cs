using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatActors.Queries.GetActorSimilarity;
using MeUi.Application.Models.ThreatActors;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatActors;

public class GetTenantActorSimilarityEndpoint : BaseTenantAuthorizedEndpoint<GetTenantActorSimilarityQuery, List<ActorSimilarityDto>, GetTenantActorSimilarityEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";
    public static string TenantPermission => "READ:TENANT_THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-actors/similarity");
        Description(x => x.WithTags("Tenant Threat Actors")
            .WithSummary("Get tenant actor similarity analysis")
            .WithDescription("Retrieves threat actor behavior correlation and clustering analysis for tenant-specific data. Requires READ:TENANT_THREAT_ACTORS permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantActorSimilarityQuery req, Guid userId, CancellationToken ct)
    {
        var similarity = await Mediator.Send(req, ct);
        await SendSuccessAsync(similarity, $"Retrieved {similarity.Count} similarity correlations for tenant {req.TenantId}", ct);
    }
}