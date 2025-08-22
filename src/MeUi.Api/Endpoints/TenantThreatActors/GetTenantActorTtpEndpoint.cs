using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatActors.Queries.GetActorTtp;
using MeUi.Application.Models.ThreatActors;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatActors;

public class GetTenantActorTtpEndpoint : BaseTenantAuthorizedEndpoint<GetTenantActorTtpQuery, List<ActorTtpDto>, GetTenantActorTtpEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";
    public static string TenantPermission => "READ:TENANT_THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-actors/ttp");
        Description(x => x.WithTags("Tenant Threat Actors")
            .WithSummary("Get tenant actor TTP analysis")
            .WithDescription("Retrieves threat actor TTP (Tactics, Techniques, Procedures) analysis for tenant-specific data. Requires READ:TENANT_THREAT_ACTORS permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantActorTtpQuery req, Guid userId, CancellationToken ct)
    {
        var ttp = await Mediator.Send(req, ct);
        await SendSuccessAsync(ttp, $"Retrieved {ttp.Count} TTP profiles for tenant {req.TenantId}", ct);
    }
}