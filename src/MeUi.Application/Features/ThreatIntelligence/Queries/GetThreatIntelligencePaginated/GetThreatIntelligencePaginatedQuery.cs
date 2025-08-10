using MediatR;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligencePaginated;

public record GetThreatIntelligencePaginatedQuery : IRequest<PaginatedThreatIntelligenceDto>
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? SearchTerm { get; init; }
    public string? Category { get; init; }
    public string? SourceCountry { get; init; }
    public string? Asn { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
    public string SortBy { get; init; } = "timestamp";
    public bool SortDescending { get; init; } = true;
}

public class PaginatedThreatIntelligenceDto
{
    public IEnumerable<ThreatRecordDto> Data { get; set; } = new List<ThreatRecordDto>();
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public class ThreatRecordDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string SourceIp { get; set; } = string.Empty;
    public string? DestinationIp { get; set; }
    public string Asn { get; set; } = string.Empty;
    public string AsnName { get; set; } = string.Empty;
    public string? SourceCountryCode { get; set; }
    public string? SourceCountryName { get; set; }
    public string? DestinationCountryCode { get; set; }
    public string? DestinationCountryName { get; set; }
    public string ThreatCategory { get; set; } = string.Empty;
    public int? SourcePort { get; set; }
    public int? DestinationPort { get; set; }
    public string? Protocol { get; set; }
    public string? MalwareFamily { get; set; }
    public string Severity { get; set; } = "Medium";
}