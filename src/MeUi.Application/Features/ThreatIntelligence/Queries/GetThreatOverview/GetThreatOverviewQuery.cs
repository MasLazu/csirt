using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatOverview;

public record GetThreatOverviewQuery : IRequest<ThreatOverviewDto>
{
    public DateTime StartDate { get; init; } = DateTime.UtcNow.AddDays(-30);
    public DateTime EndDate { get; init; } = DateTime.UtcNow;
}

public class ThreatOverviewDto
{
    public ThreatMetricsDto Metrics { get; set; } = new ThreatMetricsDto();
    public IEnumerable<ThreatTrendDto> ThreatTrends { get; set; } = new List<ThreatTrendDto>();
    public IEnumerable<TopThreatDto> TopThreats { get; set; } = new List<TopThreatDto>();
    public IEnumerable<GeographicThreatDto> GeographicDistribution { get; set; } = new List<GeographicThreatDto>();
    public DateTime LastUpdated { get; set; }
}

public class ThreatMetricsDto
{
    public long TotalThreats { get; set; }
    public long ThreatsToday { get; set; }
    public long ThreatsThisWeek { get; set; }
    public long ThreatsThisMonth { get; set; }
    public double ThreatGrowthRate { get; set; }
    public int UniqueSourceIps { get; set; }
    public int UniqueAsns { get; set; }
    public int AffectedCountries { get; set; }
    public int ThreatCategories { get; set; }
}

public class ThreatTrendDto
{
    public DateTime Date { get; set; }
    public long ThreatCount { get; set; }
    public string Period { get; set; } = string.Empty; // "hour", "day", "week"
}

public class TopThreatDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "ip", "asn", "country", "category"
    public long Count { get; set; }
    public double Percentage { get; set; }
    public string Severity { get; set; } = string.Empty;
}

public class GeographicThreatDto
{
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public long ThreatCount { get; set; }
    public double Percentage { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}