using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventTimelineAnalytics;

public record GetTenantThreatEventTimelineAnalyticsQuery : IRequest<ThreatEventTimelineAnalyticsDto>
{
    public Guid TenantId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string Interval { get; init; } = "day";
    public Guid? AsnRegistryId { get; init; }
    public string? Category { get; init; }
    public Guid? MalwareFamilyId { get; init; }
    public Guid? SourceCountryId { get; init; }
    public Guid? DestinationCountryId { get; init; }
    public bool IncludeTrends { get; init; } = true;
    public int TopItemsLimit { get; init; } = 10;
}
