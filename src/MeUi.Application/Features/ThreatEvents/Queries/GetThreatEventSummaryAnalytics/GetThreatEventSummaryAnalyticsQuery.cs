using MediatR;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventSummaryAnalytics;

public record GetThreatEventSummaryAnalyticsQuery : IRequest<ThreatEventSummaryAnalyticsDto>
{
    /// <summary>
    /// Optional time range filter (if not provided, uses last 30 days)
    /// </summary>
    public DateTime? StartTime { get; init; }

    /// <summary>
    /// Optional time range filter (if not provided, uses current time)
    /// </summary>
    public DateTime? EndTime { get; init; }

    /// <summary>
    /// Optional ASN Registry filter for tenant-specific summaries
    /// </summary>
    public Guid? AsnRegistryId { get; init; }

    /// <summary>
    /// Include critical alerts in the response
    /// </summary>
    public bool IncludeCriticalAlerts { get; init; } = true;

    /// <summary>
    /// Number of top items to include in rankings
    /// </summary>
    public int TopItemsLimit { get; init; } = 10;
}
