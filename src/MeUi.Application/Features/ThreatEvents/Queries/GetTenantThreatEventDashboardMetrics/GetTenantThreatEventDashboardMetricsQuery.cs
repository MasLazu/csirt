using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventDashboardMetrics;

public class GetTenantThreatEventDashboardMetricsQuery : IRequest<ThreatEventDashboardMetricsDto>
{
    public Guid TenantId { get; init; }
}
