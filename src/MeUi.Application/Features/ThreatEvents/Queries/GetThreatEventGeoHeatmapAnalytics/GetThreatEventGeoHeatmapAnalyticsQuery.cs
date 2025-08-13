using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventGeoHeatmapAnalytics;

public record GetThreatEventGeoHeatmapAnalyticsQuery : IRequest<ThreatEventGeoAnalyticsDto>
{
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TopCountriesLimit { get; init; } = 50;
}
