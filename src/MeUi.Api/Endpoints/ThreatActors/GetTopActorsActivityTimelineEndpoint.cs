using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatActors.Queries.GetTopActorsActivityTimeline;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Api.Endpoints.ThreatActors;

public class GetTopActorsActivityTimelineEndpoint : BaseAuthorizedEndpoint<GetTopActorsActivityTimelineQuery, List<ActorActivityTimelineDto>, GetTopActorsActivityTimelineEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-actors/activity-timeline");
        Description(x => x.WithTags("Threat Actors").WithSummary("Get top actors activity timeline"));
    }

    public override async Task HandleAuthorizedAsync(GetTopActorsActivityTimelineQuery req, Guid userId, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Count} timeline buckets", ct);
    }
}
