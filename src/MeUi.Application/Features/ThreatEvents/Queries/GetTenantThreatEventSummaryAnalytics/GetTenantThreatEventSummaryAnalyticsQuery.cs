using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventSummaryAnalytics;

public record GetTenantThreatEventSummaryAnalyticsQuery : IRequest<ThreatEventSummaryAnalyticsDto>
{
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public Guid? AsnRegistryId { get; init; }
    public bool IncludeCriticalAlerts { get; init; } = true;
    public int TopItemsLimit { get; init; } = 10;
    public Guid TenantId { get; init; }
}
