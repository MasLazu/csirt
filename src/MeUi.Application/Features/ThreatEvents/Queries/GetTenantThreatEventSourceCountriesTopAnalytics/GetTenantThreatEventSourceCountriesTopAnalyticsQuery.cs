using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventSourceCountriesTopAnalytics;

public record GetTenantThreatEventSourceCountriesTopAnalyticsQuery : IRequest<ThreatEventGeoAnalyticsDto>
{
    public Guid TenantId { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int TopLimit { get; init; } = 20;
}
