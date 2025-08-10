using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetTopCountryThreats;

public record GetTopCountryThreatsQuery : IRequest<TopCountryThreatsDto>
{
    /// <summary>
    /// Start date for country threat calculation
    /// </summary>
    public DateTime StartDate { get; init; } = DateTime.UtcNow.AddDays(-7);

    /// <summary>
    /// End date for country threat calculation
    /// </summary>
    public DateTime EndDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Limit for top country results
    /// </summary>
    public int Limit { get; init; } = 10;
}

public class TopCountryThreatsDto
{
    public IEnumerable<CountryThreatCountDto> TopCountries { get; set; } = new List<CountryThreatCountDto>();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Limit { get; set; }
    public int TotalCountries { get; set; }
}