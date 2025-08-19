using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatActors.Queries.GetActorSimilarity;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Api.Endpoints.ThreatActors;

public class GetActorSimilarityEndpoint : BaseAuthorizedEndpoint<GetActorSimilarityQuery, List<ActorSimilarityDto>, GetActorSimilarityEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-actors/similarity");
        Description(x => x.WithTags("Threat Actors").WithSummary("Get actor similarity clusters"));
    }

    public override async Task HandleAuthorizedAsync(GetActorSimilarityQuery req, Guid userId, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Count} similarity rows", ct);
    }
}
