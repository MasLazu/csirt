using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatGeographic.Queries.GetCategoryCountryTrend;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatGeographic;

public class GetCategoryCountryTrendEndpoint : BaseAuthorizedEndpoint<GetCategoryCountryTrendQuery, List<CategoryCountryTrendPointDto>, GetCategoryCountryTrendEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-geographic/top-countries-category-trend");
        Description(x => x.WithTags("Threat Geographic")
            .WithSummary("Get top countries category timeline analysis")
            .WithDescription("Retrieves category timeline for top countries. Requires READ:THREAT_GEOGRAPHIC permission."));
    }

    public override async Task HandleAuthorizedAsync(GetCategoryCountryTrendQuery req, Guid userId, CancellationToken ct)
    {
        List<CategoryCountryTrendPointDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} points", ct);
    }
}
