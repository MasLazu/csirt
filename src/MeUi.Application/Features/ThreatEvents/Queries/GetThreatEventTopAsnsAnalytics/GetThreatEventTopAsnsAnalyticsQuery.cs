using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopAsnsAnalytics;

public record GetThreatEventTopAsnsAnalyticsQuery : IRequest<ThreatEventAsnTopAnalyticsDto>
{
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TopLimit { get; init; } = 15;
}

