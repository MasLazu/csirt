using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventCategoryTimelineAnalytics;

public record GetTenantThreatEventCategoryTimelineAnalyticsQuery : IRequest<ThreatEventTimelineAnalyticsDto>
{
    public Guid TenantId { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public string Interval { get; init; } = "hour";
    public string? Category { get; init; }
}
