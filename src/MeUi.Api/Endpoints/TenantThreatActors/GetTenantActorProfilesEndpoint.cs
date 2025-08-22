using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatActors.Queries.GetActorProfiles;
using MeUi.Application.Models.ThreatActors;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatActors;

public class GetTenantActorProfilesEndpoint : BaseTenantAuthorizedEndpoint<GetTenantActorProfilesQuery, List<ActorProfileDto>, GetTenantActorProfilesEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";
    public static string TenantPermission => "READ:TENANT_THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-actors/profiles");
        Description(x => x.WithTags("Tenant Threat Actors")
            .WithSummary("Get tenant threat actor profiles")
            .WithDescription("Retrieves threat actor profiles and attribution data for tenant-specific analysis. Requires READ:TENANT_THREAT_ACTORS permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantActorProfilesQuery req, Guid userId, CancellationToken ct)
    {
        var profiles = await Mediator.Send(req, ct);
        await SendSuccessAsync(profiles, $"Retrieved {profiles.Count} actor profiles for tenant {req.TenantId}", ct);
    }
}