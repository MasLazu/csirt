using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatActors.Queries.GetActorProfiles;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Api.Endpoints.ThreatActors;

public class GetActorProfilesEndpoint : BaseAuthorizedEndpoint<GetActorProfilesQuery, List<ActorProfileDto>, GetActorProfilesEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-actors/profiles");
        Description(x => x.WithTags("Threat Actors").WithSummary("Get threat actor profiles"));
    }

    public override async Task HandleAuthorizedAsync(GetActorProfilesQuery req, Guid userId, CancellationToken ct)
    {
        List<ActorProfileDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Count} actor profiles", ct);
    }
}
