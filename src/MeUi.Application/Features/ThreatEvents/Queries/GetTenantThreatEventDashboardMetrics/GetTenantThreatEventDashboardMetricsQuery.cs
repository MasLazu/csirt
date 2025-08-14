
using MediatR;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventDashboardMetrics;

public class GetTenantThreatEventDashboardMetricsQuery : IRequest<ThreatEventDashboardMetricsDto>, ITenantRequest
{
    public Guid TenantId { get; set; }
}
