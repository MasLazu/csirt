using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventOverviewAnalytics;

public record GetTenantThreatEventOverviewAnalyticsQuery : IRequest<ThreatEventOverviewAnalyticsDto>
{
    public Guid TenantId { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TopItemsLimit { get; init; } = 5;
}
