using MediatR;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopPortsAnalytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventTopPortsAnalytics;

public record GetTenantThreatEventTopPortsAnalyticsQuery : IRequest<ThreatEventPortTopAnalyticsDto>
{
    public Guid TenantId { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int Top { get; init; } = 20;
    public bool IncludeDestination { get; init; } = true;
}
