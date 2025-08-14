
using MediatR;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventCategoryTimelineAnalytics;

public record GetTenantThreatEventCategoryTimelineAnalyticsQuery : IRequest<ThreatEventTimelineAnalyticsDto>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public string Interval { get; init; } = "hour";
    public string? Category { get; init; }
}
