using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatActors.Queries.GetActorPersistence;
using MeUi.Application.Models.ThreatActors;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatActors;

public class GetTenantActorPersistenceEndpoint : BaseTenantAuthorizedEndpoint<GetTenantActorPersistenceQuery, List<ActorPersistenceDto>, GetTenantActorPersistenceEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";
    public static string TenantPermission => "READ:TENANT_THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-actors/persistence");
        Description(x => x.WithTags("Tenant Threat Actors")
            .WithSummary("Get tenant actor persistence patterns")
            .WithDescription("Retrieves threat actor persistence pattern analysis for tenant-specific data. Requires READ:TENANT_THREAT_ACTORS permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantActorPersistenceQuery req, Guid userId, CancellationToken ct)
    {
        var persistence = await Mediator.Send(req, ct);
        await SendSuccessAsync(persistence, $"Retrieved {persistence.Count} persistence patterns for tenant {req.TenantId}", ct);
    }
}