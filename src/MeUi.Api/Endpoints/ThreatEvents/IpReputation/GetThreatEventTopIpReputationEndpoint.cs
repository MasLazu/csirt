using FastEndpoints;
using MediatR;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopIpReputationAnalytics;

namespace MeUi.Api.Endpoints.ThreatEvents.IpReputation;

public class GetThreatEventTopIpReputationRequest
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int Top { get; set; } = 20;
    public bool IncludeDestination { get; set; } = true;
}

public class GetThreatEventTopIpReputationEndpoint : Endpoint<GetThreatEventTopIpReputationRequest>
{
    private readonly IMediator _mediator;

    public GetThreatEventTopIpReputationEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/v1/threat-events/analytics/ip-reputation/top");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get top source (and optionally destination) IP addresses by reputation frequency in threat events.";
            s.Description = "Returns the most frequent source IPs (and destination IPs if requested) participating in threat events within the time window.";
        });
    }

    public override async Task HandleAsync(GetThreatEventTopIpReputationRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetThreatEventTopIpReputationAnalyticsQuery
        {
            StartTime = req.StartTime,
            EndTime = req.EndTime,
            Top = req.Top,
            IncludeDestination = req.IncludeDestination
        }, ct);

        await SendOkAsync(result, ct);
    }
}
