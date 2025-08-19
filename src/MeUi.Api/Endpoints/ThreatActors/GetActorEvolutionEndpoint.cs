using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatActors.Queries.GetActorEvolution;
using MeUi.Application.Models.ThreatActors;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatActors;

public class GetActorEvolutionEndpoint : BaseAuthorizedEndpoint<GetActorEvolutionQuery, List<ActorEvolutionDto>, GetActorEvolutionEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-actors/evolution");
        Description(x => x.WithTags("Threat Actors").WithSummary("Get actor evolution metrics"));
    }

    public override async Task HandleAuthorizedAsync(GetActorEvolutionQuery req, Guid userId, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Count} evolution rows", ct);
    }
}
