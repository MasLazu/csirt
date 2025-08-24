using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatTemporal.Queries.Get24HourAttackPattern;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Api.Endpoints.ThreatTemporal;

public class Get24HourAttackPatternEndpoint : BaseAuthorizedEndpoint<Get24HourAttackPatternQuery, List<TimeSeriesPointDto>, Get24HourAttackPatternEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-temporal/24-hour-pattern");
        Description(x => x.WithTags("Threat Temporal").WithSummary("24-hour attack pattern").WithDescription("Get events grouped by hour."));
    }

    public override async Task HandleAuthorizedAsync(Get24HourAttackPatternQuery req, Guid userId, CancellationToken ct)
    {
        List<TimeSeriesPointDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} points", ct);
    }
}
