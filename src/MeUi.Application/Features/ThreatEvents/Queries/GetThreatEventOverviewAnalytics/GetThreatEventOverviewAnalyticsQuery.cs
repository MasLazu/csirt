using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventOverviewAnalytics;

/// <summary>
/// Request for lightweight overview analytics. Defaults to last 30 days.
/// </summary>
public record GetThreatEventOverviewAnalyticsQuery : IRequest<ThreatEventOverviewAnalyticsDto>
{
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TopItemsLimit { get; init; } = 5; // smaller for quick load
}

