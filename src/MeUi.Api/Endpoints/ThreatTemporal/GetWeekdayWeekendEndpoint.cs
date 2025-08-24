using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatTemporal.Queries.GetWeekdayWeekend;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Api.Endpoints.ThreatTemporal;

public class GetWeekdayWeekendEndpoint : BaseAuthorizedEndpoint<GetWeekdayWeekendQuery, List<TimeSeriesPointDto>, GetWeekdayWeekendEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-temporal/weekday-weekend-trends");
        Description(x => x.WithTags("Threat Temporal").WithSummary("Weekday vs Weekend trends").WithDescription("Returns daily time-series grouped by weekday/weekend."));
    }

    public override async Task HandleAuthorizedAsync(GetWeekdayWeekendQuery req, Guid userId, CancellationToken ct)
    {
        List<TimeSeriesPointDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} points", ct);
    }
}
