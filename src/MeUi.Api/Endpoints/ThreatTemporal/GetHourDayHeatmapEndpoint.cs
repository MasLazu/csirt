using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatTemporal.Queries.GetHourDayHeatmap;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Api.Endpoints.ThreatTemporal;

public class GetHourDayHeatmapEndpoint : BaseAuthorizedEndpoint<GetHourDayHeatmapQuery, List<HourDayHeatmapDto>, GetHourDayHeatmapEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-temporal/hour-day-heatmap");
        Description(x => x.WithTags("Threat Temporal").WithSummary("Hour vs Day heatmap").WithDescription("Returns heatmap points"));
    }

    public override async Task HandleAuthorizedAsync(GetHourDayHeatmapQuery req, Guid userId, CancellationToken ct)
    {
        List<HourDayHeatmapDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} points", ct);
    }
}
