using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTimelineAnalytics;

public record GetThreatEventTimelineAnalyticsQuery : IRequest<ThreatEventTimelineAnalyticsDto>
{
    /// <summary>
    /// Start time for analytics (required for performance)
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// End time for analytics (required for performance)
    /// </summary>
    public DateTime EndTime { get; init; }

    /// <summary>
    /// Time bucket interval: hour, day, week, month
    /// </summary>
    public string Interval { get; init; } = "day";

    /// <summary>
    /// Optional ASN Registry filter
    /// </summary>
    public Guid? AsnRegistryId { get; init; }

    /// <summary>
    /// Optional category filter
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Optional malware family filter
    /// </summary>
    public Guid? MalwareFamilyId { get; init; }

    /// <summary>
    /// Optional source country filter
    /// </summary>
    public Guid? SourceCountryId { get; init; }

    /// <summary>
    /// Optional destination country filter
    /// </summary>
    public Guid? DestinationCountryId { get; init; }

    /// <summary>
    /// Include trend analysis comparison
    /// </summary>
    public bool IncludeTrends { get; init; } = true;

    /// <summary>
    /// Maximum number of top categories/malware families to include in breakdown
    /// </summary>
    public int TopItemsLimit { get; init; } = 10;
}
