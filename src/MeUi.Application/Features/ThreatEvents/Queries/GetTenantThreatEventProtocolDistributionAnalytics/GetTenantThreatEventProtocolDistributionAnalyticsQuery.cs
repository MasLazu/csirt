using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventProtocolDistributionAnalytics;

public record GetTenantThreatEventProtocolDistributionAnalyticsQuery : IRequest<ThreatEventProtocolDistributionAnalyticsDto>
{
    public Guid TenantId { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
}
