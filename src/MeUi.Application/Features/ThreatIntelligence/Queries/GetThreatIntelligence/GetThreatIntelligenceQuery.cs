using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligence;

public record GetThreatIntelligenceQuery : IRequest<ThreatIntelligenceListDto>
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Category { get; init; }
    public string? SourceCountry { get; init; }
    public string? Asn { get; init; }
    public string? SourceAddress { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortBy { get; init; } = "timestamp";
    public bool SortDescending { get; init; } = true;
}

public class ThreatIntelligenceListDto
{
    public IEnumerable<ThreatIntelligenceItemDto> Items { get; set; } = new List<ThreatIntelligenceItemDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class ThreatIntelligenceItemDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string SourceAddress { get; set; } = string.Empty;
    public string? DestinationAddress { get; set; }
    public string Asn { get; set; } = string.Empty;
    public string AsnDescription { get; set; } = string.Empty;
    public string? SourceCountry { get; set; }
    public string? DestinationCountry { get; set; }
    public string Category { get; set; } = string.Empty;
    public int? SourcePort { get; set; }
    public int? DestinationPort { get; set; }
    public string? Protocol { get; set; }
    public string? MalwareFamily { get; set; }
}