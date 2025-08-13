using MediatR;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopIpReputationAnalytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventTopIpReputationAnalytics;

public record GetTenantThreatEventTopIpReputationAnalyticsQuery : IRequest<ThreatEventTopIpReputationAnalyticsDto>
{
    public Guid TenantId { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int Top { get; init; } = 20;
    public bool IncludeDestination { get; init; } = true;
}
