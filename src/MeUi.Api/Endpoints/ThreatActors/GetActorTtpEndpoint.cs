using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatActors.Queries.GetActorTtp;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Api.Endpoints.ThreatActors;

public class GetActorTtpEndpoint : BaseAuthorizedEndpoint<GetActorTtpQuery, List<ActorTtpDto>, GetActorTtpEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-actors/ttp-profile");
        Description(x => x.WithTags("Threat Actors").WithSummary("Get actor TTP profiles"));
    }

    public override async Task HandleAuthorizedAsync(GetActorTtpQuery req, Guid userId, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Count} TTP profiles", ct);
    }
}
