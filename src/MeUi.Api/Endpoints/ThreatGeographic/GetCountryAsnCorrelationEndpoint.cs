using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatGeographic.Queries.GetCountryAsnCorrelation;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatGeographic;

public class GetCountryAsnCorrelationEndpoint : BaseAuthorizedEndpoint<GetCountryAsnCorrelationQuery, List<CountryAsnCorrelationDto>, GetCountryAsnCorrelationEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-geographic/country-asn-correlation");
        Description(x => x.WithTags("Threat Geographic")
            .WithSummary("Get country-ASN correlation matrix")
            .WithDescription("Retrieves country-ASN event correlations. Requires READ:THREAT_GEOGRAPHIC permission."));
    }

    public override async Task HandleAuthorizedAsync(GetCountryAsnCorrelationQuery req, Guid userId, CancellationToken ct)
    {
        List<CountryAsnCorrelationDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} rows", ct);
    }
}
