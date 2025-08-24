using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatTemporal.Queries.GetPeakActivity;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Api.Endpoints.ThreatTemporal;

public class GetPeakActivityEndpoint : BaseAuthorizedEndpoint<GetPeakActivityQuery, List<PeakActivityDto>, GetPeakActivityEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-temporal/peak-activity");
        Description(x => x.WithTags("Threat Temporal").WithSummary("Peak activity by category").WithDescription("Returns top activity hours broken down by category."));
    }

    public override async Task HandleAuthorizedAsync(GetPeakActivityQuery req, Guid userId, CancellationToken ct)
    {
        List<PeakActivityDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} rows", ct);
    }
}
