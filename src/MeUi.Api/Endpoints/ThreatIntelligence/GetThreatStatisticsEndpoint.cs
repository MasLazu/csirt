using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatStatistics;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetThreatStatisticsEndpoint : BaseEndpoint<GetThreatStatisticsQuery, ThreatStatisticsDto>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/statistics");
        AuthSchemes("Bearer");
        Description(x => x
            .WithTags("Threat Intelligence")
            .WithSummary("Get threat intelligence statistics")
            .WithDescription("Retrieve aggregated threat intelligence statistics using TimescaleDB time-series functions"));
    }

    public override async Task HandleAsync(GetThreatStatisticsQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Threat intelligence statistics retrieved successfully", ct);
    }
}