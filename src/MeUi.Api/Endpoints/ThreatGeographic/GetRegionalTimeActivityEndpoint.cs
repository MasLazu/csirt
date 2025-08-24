using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatGeographic.Queries.GetRegionalTimeActivity;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatGeographic;

public class GetRegionalTimeActivityEndpoint : BaseAuthorizedEndpoint<GetRegionalTimeActivityQuery, List<RegionalTimeBucketDto>, GetRegionalTimeActivityEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-geographic/regional-time-activity");
        Description(x => x.WithTags("Threat Geographic")
            .WithSummary("Get regional time zone activity")
            .WithDescription("Retrieves event counts by regional time buckets. Requires READ:THREAT_GEOGRAPHIC permission."));
    }

    public override async Task HandleAuthorizedAsync(GetRegionalTimeActivityQuery req, Guid userId, CancellationToken ct)
    {
        List<RegionalTimeBucketDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} buckets", ct);
    }
}
