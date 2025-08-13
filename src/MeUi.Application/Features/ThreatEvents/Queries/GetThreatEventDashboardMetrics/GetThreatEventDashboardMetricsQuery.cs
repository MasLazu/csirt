using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventDashboardMetrics;

public class GetThreatEventDashboardMetricsQuery : IRequest<ThreatEventDashboardMetricsDto>
{
    // Swagger requires at least one public property on request DTOs; this is a no-op optional marker.
    public bool IncludeExtended { get; init; } = false;
}
