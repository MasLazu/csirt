namespace MeUi.Application.Models.Analytics;

/// <summary>
/// Geographic distribution analytics for threat events
/// </summary>
public class ThreatEventGeoAnalyticsDto
{
    public List<CountryThreatStatsDto> SourceCountries { get; set; } = new();
    public List<CountryThreatStatsDto> DestinationCountries { get; set; } = new();
    public List<ThreatFlowDto> ThreatFlows { get; set; } = new();
    public int TotalUniqueCountries { get; set; }
    public TimeRangeDto TimeRange { get; set; } = new();
}

/// <summary>
/// Country-specific threat statistics
/// </summary>
public class CountryThreatStatsDto
{
    public Guid? CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int EventCount { get; set; }
    public double Percentage { get; set; }
    public List<string> TopCategories { get; set; } = new();
    public List<string> TopMalwareFamilies { get; set; } = new();
}

/// <summary>
/// Cross-border threat flow data
/// </summary>
public class ThreatFlowDto
{
    public string SourceCountryCode { get; set; } = string.Empty;
    public string DestinationCountryCode { get; set; } = string.Empty;
    public string SourceCountryName { get; set; } = string.Empty;
    public string DestinationCountryName { get; set; } = string.Empty;
    public int EventCount { get; set; }
    public string PrimaryCategory { get; set; } = string.Empty;
    public double ThreatIntensity { get; set; } // Calculated threat score
}
