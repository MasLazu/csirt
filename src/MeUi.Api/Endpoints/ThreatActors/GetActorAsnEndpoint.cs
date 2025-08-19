using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatActors.Queries.GetActorAsn;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Api.Endpoints.ThreatActors;

public class GetActorAsnEndpoint : BaseAuthorizedEndpoint<GetActorAsnQuery, List<ActorAsnDto>, GetActorAsnEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-actors/asns");
        Description(x => x.WithTags("Threat Actors").WithSummary("Get most active ASNs for actors"));
    }

    public override async Task HandleAuthorizedAsync(GetActorAsnQuery req, Guid userId, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Count} ASN buckets", ct);
    }
}
