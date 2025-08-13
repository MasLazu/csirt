using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventGeoHeatmapAnalytics;

public record GetTenantThreatEventGeoHeatmapAnalyticsQuery : IRequest<ThreatEventGeoAnalyticsDto>
{
    public Guid TenantId { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TopCountriesLimit { get; init; } = 50;
}
