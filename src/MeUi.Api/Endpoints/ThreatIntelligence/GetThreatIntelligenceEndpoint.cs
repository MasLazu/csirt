using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligence;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetThreatIntelligenceEndpoint : BaseEndpoint<GetThreatIntelligenceQuery, IEnumerable<ThreatIntelligenceDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence");
        Description(x => x
            .WithTags("Threat Intelligence")
            .WithSummary("Get filtered threat intelligence data")
            .WithDescription("Retrieve threat intelligence data based on various filter criteria"));
    }

    public override async Task HandleAsync(GetThreatIntelligenceQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Threat intelligence data retrieved successfully", ct);
    }
}