using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligencePaginated;
using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.ThreatIntelligence;

public class GetThreatIntelligencePaginatedEndpoint : BaseEndpoint<GetThreatIntelligencePaginatedQuery, PaginatedResult<ThreatIntelligenceDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligence/paginated");
        Description(x => x
            .WithTags("Threat Intelligence")
            .WithSummary("Get paginated threat intelligence data")
            .WithDescription("Retrieve paginated threat intelligence data with filtering and sorting capabilities"));
    }

    public override async Task HandleAsync(GetThreatIntelligencePaginatedQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Paginated threat intelligence data retrieved successfully", ct);
    }
}