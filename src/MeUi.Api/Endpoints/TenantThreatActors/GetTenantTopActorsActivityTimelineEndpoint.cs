using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatActors.Queries.GetTopActorsActivityTimeline;
using MeUi.Application.Models.ThreatActors;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatActors;

public class GetTenantTopActorsActivityTimelineEndpoint : BaseTenantAuthorizedEndpoint<GetTenantTopActorsActivityTimelineQuery, List<ActorActivityTimelineDto>, GetTenantTopActorsActivityTimelineEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";
    public static string TenantPermission => "READ:TENANT_THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-actors/activity-timeline");
        Description(x => x.WithTags("Tenant Threat Actors")
            .WithSummary("Get tenant top actors activity timeline")
            .WithDescription("Retrieves activity timeline for top threat actors in tenant-specific data. Requires READ:TENANT_THREAT_ACTORS permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantTopActorsActivityTimelineQuery req, Guid userId, CancellationToken ct)
    {
        var timeline = await Mediator.Send(req, ct);
        await SendSuccessAsync(timeline, $"Retrieved {timeline.Count} timeline entries for tenant {req.TenantId}", ct);
    }
}