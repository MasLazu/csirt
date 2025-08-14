
using MediatR;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventProtocolDistributionAnalytics;

public record GetTenantThreatEventProtocolDistributionAnalyticsQuery : IRequest<ThreatEventProtocolDistributionAnalyticsDto>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
}
