
using MediatR;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventTopAsnsAnalytics;

public record GetTenantThreatEventTopAsnsAnalyticsQuery : IRequest<ThreatEventAsnTopAnalyticsDto>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TopLimit { get; init; } = 15;
}
