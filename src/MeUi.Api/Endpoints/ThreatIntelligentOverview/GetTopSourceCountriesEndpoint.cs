using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetTopSourceCountries;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.ThreatIntelligentOverview;

public class GetTopSourceCountriesEndpoint : BaseAuthorizedEndpoint<GetTopSourceCountriesQuery, List<TopCountryDto>, GetTopSourceCountriesEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligent-overview/top-source-countries");
        Description(x => x.WithTags("Threat Intelligent Overview")
            .WithSummary("Get top source countries")
            .WithDescription("Retrieves top source countries for threat intelligence overview. Requires READ:THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    public override async Task HandleAuthorizedAsync(GetTopSourceCountriesQuery req, Guid userId, CancellationToken ct)
    {
        var countries = await Mediator.Send(req, ct);
        await SendSuccessAsync(countries, $"Retrieved {countries.Count} top source countries", ct);
    }
}
