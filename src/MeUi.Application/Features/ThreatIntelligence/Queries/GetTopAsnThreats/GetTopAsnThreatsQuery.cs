using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetTopAsnThreats;

public record GetTopAsnThreatsQuery : IRequest<TopAsnThreatsDto>
{
    /// <summary>
    /// Start date for ASN threat calculation
    /// </summary>
    public DateTime StartDate { get; init; } = DateTime.UtcNow.AddDays(-7);

    /// <summary>
    /// End date for ASN threat calculation
    /// </summary>
    public DateTime EndDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Limit for top ASN results
    /// </summary>
    public int Limit { get; init; } = 10;
}

public class TopAsnThreatsDto
{
    public IEnumerable<AsnThreatCountDto> TopAsns { get; set; } = new List<AsnThreatCountDto>();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Limit { get; set; }
    public int TotalAsns { get; set; }
}