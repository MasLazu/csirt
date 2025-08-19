using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatActors.Queries.GetActorDistributionByCountry;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Api.Endpoints.ThreatActors;

public class GetActorDistributionByCountryEndpoint : BaseAuthorizedEndpoint<GetActorDistributionByCountryQuery, List<ActorCountryDistributionDto>, GetActorDistributionByCountryEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_ACTORS";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-actors/distribution-by-country");
        Description(x => x.WithTags("Threat Actors").WithSummary("Get actor distribution by country"));
    }

    public override async Task HandleAuthorizedAsync(GetActorDistributionByCountryQuery req, Guid userId, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Count} country buckets", ct);
    }
}
