using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventProtocolDistributionAnalytics;

public record GetThreatEventProtocolDistributionAnalyticsQuery : IRequest<ThreatEventProtocolDistributionAnalyticsDto>
{
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
}
