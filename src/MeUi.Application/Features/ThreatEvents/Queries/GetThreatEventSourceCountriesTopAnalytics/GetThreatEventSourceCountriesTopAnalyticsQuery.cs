using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventSourceCountriesTopAnalytics;

public record GetThreatEventSourceCountriesTopAnalyticsQuery : IRequest<ThreatEventGeoAnalyticsDto>
{
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TopLimit { get; init; } = 20;
}
