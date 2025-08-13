using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopPortsAnalytics;

public record GetThreatEventTopPortsAnalyticsQuery : IRequest<ThreatEventPortTopAnalyticsDto>
{
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int Top { get; init; } = 20;
    public bool IncludeDestination { get; init; } = true; // if true returns both source and destination lists
}

public class ThreatEventPortTopAnalyticsDto
{
    public List<PortAnalytics> TopSourcePorts { get; set; } = new();
    public List<PortAnalytics> TopDestinationPorts { get; set; } = new();
    public TimeRangeDto TimeRange { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
