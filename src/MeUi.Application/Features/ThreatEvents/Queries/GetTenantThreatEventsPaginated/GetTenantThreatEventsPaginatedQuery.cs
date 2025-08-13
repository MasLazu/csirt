using System.Net;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventsPaginated;

public record GetTenantThreatEventsPaginatedQuery : BasePaginatedQuery<ThreatEventDto>
{
    public Guid TenantId { get; init; }

    // Time range filtering (critical for TimescaleDB performance)
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }

    // Location filtering
    public Guid? SourceCountryId { get; init; }
    public Guid? DestinationCountryId { get; init; }

    // Network filtering
    public IPAddress? SourceAddress { get; init; }
    public IPAddress? DestinationAddress { get; init; }
    public int? SourcePort { get; init; }
    public int? DestinationPort { get; init; }
    public Guid? ProtocolId { get; init; }

    // Threat classification
    public string? Category { get; init; }
    public Guid? MalwareFamilyId { get; init; }

    public GetTenantThreatEventsPaginatedQuery()
    {
        SortBy = "Timestamp"; // Default sort by timestamp for time-series data
        SortDirection = "desc"; // Most recent first
    }
}
