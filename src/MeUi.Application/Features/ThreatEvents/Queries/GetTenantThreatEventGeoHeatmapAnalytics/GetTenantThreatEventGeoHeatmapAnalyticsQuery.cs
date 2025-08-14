
using MediatR;
using MeUi.Application.Models.Analytics;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventGeoHeatmapAnalytics;

public record GetTenantThreatEventGeoHeatmapAnalyticsQuery : IRequest<ThreatEventGeoAnalyticsDto>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TopCountriesLimit { get; init; } = 50;
}
