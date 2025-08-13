using MediatR;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Interfaces;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopIpReputationAnalytics;

public record GetThreatEventTopIpReputationAnalyticsQuery : IRequest<ThreatEventTopIpReputationAnalyticsDto>
{
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int Top { get; init; } = 20;
    public bool IncludeDestination { get; init; } = true; // include destination IPs list
}

public class ThreatEventTopIpReputationAnalyticsDto
{
    public List<IpReputationAnalytics> TopSourceIps { get; set; } = new();
    public List<IpReputationAnalytics> TopDestinationIps { get; set; } = new();
    public TimeRangeDto TimeRange { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
