namespace MeUi.Application.Models.Analytics;

/// <summary>
/// Lightweight threat event overview for fast dashboard widgets.
/// Combines key volume, diversity and top contributor metrics.
/// </summary>
public class ThreatEventOverviewAnalyticsDto
{
    public ThreatEventOverviewStatsDto Overview { get; set; } = new();
    public List<TopItemDto> TopCategories { get; set; } = new();
    public List<TopItemDto> TopMalwareFamilies { get; set; } = new();
    public List<TopItemDto> TopSourceCountries { get; set; } = new();
    public double PercentageChangeFromYesterday { get; set; }
    public DateTime LastUpdated { get; set; }
    public TimeRangeDto TimeRange { get; set; } = new();
}

/// <summary>
/// Core high-level stats (subset of full summary for speed)
/// </summary>
public class ThreatEventOverviewStatsDto
{
    public int TotalEvents { get; set; }
    public int UniqueSourceIps { get; set; }
    public int UniqueMalwareFamilies { get; set; }
    public int UniqueCountries { get; set; }
    public int EventsLast24Hours { get; set; }
    public int EventsLastHour { get; set; }
    public int ActiveThreatsCurrently { get; set; }
    public int CriticalAlertsCount { get; set; }
    public string TopThreatCategory { get; set; } = string.Empty;
    public string TopSourceCountry { get; set; } = string.Empty;
}
