using FastEndpoints;
using MediatR;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopPortsAnalytics;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatEvents.Ports;

public class GetThreatEventTopPortsRequest
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int Top { get; set; } = 20;
    public bool IncludeDestination { get; set; } = true;
}

public class GetThreatEventTopPortsEndpoint : Endpoint<GetThreatEventTopPortsRequest>
{
    private readonly IMediator _mediator;

    public GetThreatEventTopPortsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/v1/threat-events/analytics/ports/top");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get top source (and optionally destination) ports involved in threat events for the specified window.";
            s.Description = "Returns the most frequent ports within the given time range. When IncludeDestination=true, includes destination port list.";
        });
    }

    public override async Task HandleAsync(GetThreatEventTopPortsRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetThreatEventTopPortsAnalyticsQuery
        {
            StartTime = req.StartTime,
            EndTime = req.EndTime,
            Top = req.Top,
            IncludeDestination = req.IncludeDestination
        }, ct);

        await SendOkAsync(result, ct);
    }
}
