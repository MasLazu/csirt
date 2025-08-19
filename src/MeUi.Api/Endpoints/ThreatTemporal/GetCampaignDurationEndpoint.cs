using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatTemporal.Queries.GetCampaignDuration;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Api.Endpoints.ThreatTemporal;

public class GetCampaignDurationEndpoint : BaseAuthorizedEndpoint<GetCampaignDurationQuery, List<CampaignDurationDto>, GetCampaignDurationEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-temporal/campaign-duration");
        Description(x => x.WithTags("Threat Temporal").WithSummary("Attack campaign duration").WithDescription("Returns campaign duration metrics."));
    }

    public override async Task HandleAuthorizedAsync(GetCampaignDurationQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} campaigns", ct);
    }
}
