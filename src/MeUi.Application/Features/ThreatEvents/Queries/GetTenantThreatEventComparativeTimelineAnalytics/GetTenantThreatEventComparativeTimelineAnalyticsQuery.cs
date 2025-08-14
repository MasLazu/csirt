
using MediatR;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventComparativeTimelineAnalytics;

public record GetTenantThreatEventComparativeTimelineAnalyticsQuery : IRequest<ThreatEventTimelineAnalyticsDto>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime? CurrentStart { get; init; }
    public DateTime? CurrentEnd { get; init; }
    public DateTime? PreviousStart { get; init; }
    public DateTime? PreviousEnd { get; init; }
    public string Interval { get; init; } = "1h"; // 5m|15m|1h|6h|1d|1w
}
