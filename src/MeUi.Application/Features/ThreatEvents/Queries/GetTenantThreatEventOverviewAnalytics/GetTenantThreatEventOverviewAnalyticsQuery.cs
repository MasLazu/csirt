
using MediatR;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventOverviewAnalytics;

public record GetTenantThreatEventOverviewAnalyticsQuery : IRequest<ThreatEventOverviewAnalyticsDto>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TopItemsLimit { get; init; } = 5;
}
