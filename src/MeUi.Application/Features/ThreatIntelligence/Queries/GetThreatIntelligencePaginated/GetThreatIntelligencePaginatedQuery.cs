using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligencePaginated;

public record GetThreatIntelligencePaginatedQuery : IRequest<PaginatedResult<ThreatIntelligenceDto>>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Filter by Autonomous System Number (ASN)
    /// </summary>
    public string? Asn { get; init; }

    /// <summary>
    /// Filter by source IP address
    /// </summary>
    public string? SourceAddress { get; init; }

    /// <summary>
    /// Filter by destination IP address
    /// </summary>
    public string? DestinationAddress { get; init; }

    /// <summary>
    /// Filter by source country code
    /// </summary>
    public string? SourceCountry { get; init; }

    /// <summary>
    /// Filter by destination country code
    /// </summary>
    public string? DestinationCountry { get; init; }

    /// <summary>
    /// Filter by threat category
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Filter by network protocol (TCP, UDP, etc.)
    /// </summary>
    public string? Protocol { get; init; }

    /// <summary>
    /// Filter by source port number
    /// </summary>
    public int? SourcePort { get; init; }

    /// <summary>
    /// Filter by destination port number
    /// </summary>
    public int? DestinationPort { get; init; }

    /// <summary>
    /// Filter by malware family name
    /// </summary>
    public string? MalwareFamily { get; init; }

    /// <summary>
    /// Filter records from this date onwards (inclusive)
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Filter records up to this date (inclusive)
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Field to sort results by
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort in descending order (default: true)
    /// </summary>
    public bool SortDescending { get; init; } = true;
}