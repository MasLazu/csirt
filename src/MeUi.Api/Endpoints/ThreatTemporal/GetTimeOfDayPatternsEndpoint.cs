using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatTemporal.Queries.GetTimeOfDayPatterns;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Api.Endpoints.ThreatTemporal;

public class GetTimeOfDayPatternsEndpoint : BaseAuthorizedEndpoint<GetTimeOfDayPatternsQuery, List<TimePeriodSeriesDto>, GetTimeOfDayPatternsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-temporal/time-of-day-patterns");
        Description(x => x.WithTags("Threat Temporal").WithSummary("Attack patterns by time of day").WithDescription("Returns series grouped by morning/afternoon/evening/night."));
    }

    public override async Task HandleAuthorizedAsync(GetTimeOfDayPatternsQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} series", ct);
    }
}
