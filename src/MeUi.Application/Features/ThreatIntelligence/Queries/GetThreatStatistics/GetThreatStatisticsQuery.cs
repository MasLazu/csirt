using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatStatistics;

public record GetThreatStatisticsQuery : IRequest<ThreatStatisticsDto>
{
    /// <summary>
    /// Start date for statistics calculation
    /// </summary>
    public DateTime StartDate { get; init; } = DateTime.UtcNow.AddDays(-7);

    /// <summary>
    /// End date for statistics calculation
    /// </summary>
    public DateTime EndDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Time bucket interval (hour, day)
    /// </summary>
    public string Interval { get; init; } = "hour";

    /// <summary>
    /// Limit for top results (ASN, country, etc.)
    /// </summary>
    public int Limit { get; init; } = 10;
}

public class ThreatStatisticsDto
{
    public IEnumerable<HourlyThreatCountDto> HourlyThreatCounts { get; set; } = new List<HourlyThreatCountDto>();
    public IEnumerable<DailyThreatCountDto> DailyThreatCounts { get; set; } = new List<DailyThreatCountDto>();
    public IEnumerable<AsnThreatCountDto> TopAsnThreats { get; set; } = new List<AsnThreatCountDto>();
    public IEnumerable<CountryThreatCountDto> TopCountryThreats { get; set; } = new List<CountryThreatCountDto>();
    public IEnumerable<CategoryThreatCountDto> CategoryBreakdown { get; set; } = new List<CategoryThreatCountDto>();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalThreats { get; set; }
}