using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventComparativeTimelineAnalytics;

public record GetThreatEventComparativeTimelineAnalyticsQuery : IRequest<ThreatEventTimelineAnalyticsDto>
{
    public DateTime? CurrentStart { get; init; }
    public DateTime? CurrentEnd { get; init; }
    public DateTime? PreviousStart { get; init; }
    public DateTime? PreviousEnd { get; init; }
    public string Interval { get; init; } = "1h"; // 5m|15m|1h|6h|1d|1w
}
