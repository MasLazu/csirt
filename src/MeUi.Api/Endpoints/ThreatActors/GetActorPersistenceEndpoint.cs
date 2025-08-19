using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatActors.Queries.GetActorPersistence;
using MeUi.Application.Models.ThreatActors;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatActors;

public class GetActorPersistenceEndpoint : BaseAuthorizedEndpoint<GetActorPersistenceQuery, List<ActorPersistenceDto>, GetActorPersistenceEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-actors/persistence");
        Description(x => x.WithTags("Threat Actors").WithSummary("Get actor persistence patterns"));
    }

    public override async Task HandleAuthorizedAsync(GetActorPersistenceQuery req, Guid userId, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Count} persistence buckets", ct);
    }
}
