using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatActors.Queries.GetActorEvolution;
using MeUi.Application.Models.ThreatActors;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatActors;

public class GetTenantActorEvolutionEndpoint : BaseTenantAuthorizedEndpoint<GetTenantActorEvolutionQuery, List<ActorEvolutionDto>, GetTenantActorEvolutionEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";
    public static string TenantPermission => "READ:TENANT_THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-actors/evolution");
        Description(x => x.WithTags("Tenant Threat Actors")
            .WithSummary("Get tenant actor campaign evolution")
            .WithDescription("Retrieves threat actor campaign evolution and intensity analysis for tenant-specific data. Requires READ:TENANT_THREAT_ACTORS permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantActorEvolutionQuery req, Guid userId, CancellationToken ct)
    {
        var evolution = await Mediator.Send(req, ct);
        await SendSuccessAsync(evolution, $"Retrieved {evolution.Count} evolution metrics for tenant {req.TenantId}", ct);
    }
}