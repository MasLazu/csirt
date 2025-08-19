using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatGeographic.Queries.GetCountryAttackTrends;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatGeographic;

public class GetCountryAttackTrendsEndpoint : BaseAuthorizedEndpoint<GetCountryAttackTrendsQuery, List<CountryAttackTrendPointDto>, GetCountryAttackTrendsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-geographic/country-attack-trends");
        Description(x => x.WithTags("Threat Geographic")
            .WithSummary("Get country attack trends over time")
            .WithDescription("Retrieves time series of attack counts per country. Requires READ:THREAT_GEOGRAPHIC permission."));
    }

    public override async Task HandleAuthorizedAsync(GetCountryAttackTrendsQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} trend points", ct);
    }
}
