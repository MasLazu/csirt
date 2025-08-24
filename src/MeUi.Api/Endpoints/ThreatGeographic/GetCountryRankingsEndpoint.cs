using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatGeographic.Queries.GetCountryRankings;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatGeographic;

public class GetCountryRankingsEndpoint : BaseAuthorizedEndpoint<GetCountryRankingsQuery, List<CountryAttackRankingDto>, GetCountryRankingsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-geographic/country-rankings");
        Description(x => x.WithTags("Threat Geographic")
            .WithSummary("Get country attack rankings")
            .WithDescription("Retrieves top countries by attack events. Requires READ:THREAT_GEOGRAPHIC permission."));
    }

    public override async Task HandleAuthorizedAsync(GetCountryRankingsQuery req, Guid userId, CancellationToken ct)
    {
        List<CountryAttackRankingDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} countries", ct);
    }
}
