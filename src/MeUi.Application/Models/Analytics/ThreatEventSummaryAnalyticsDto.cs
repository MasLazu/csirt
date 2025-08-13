namespace MeUi.Application.Models.Analytics;

/// <summary>
/// Comprehensive threat event summary for executive dashboards
/// </summary>
public class ThreatEventSummaryAnalyticsDto
{
    public ThreatOverviewDto Overview { get; set; } = new();
    public TrendIndicatorsDto TrendIndicators { get; set; } = new();
    public Dictionary<string, CategoryStatsDto> ThreatCategories { get; set; } = new();
    public List<CriticalAlertDto> CriticalAlerts { get; set; } = new();
    public TopThreatsDto TopThreats { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// High-level threat overview statistics
/// </summary>
public class ThreatOverviewDto
{
    public int TotalEvents { get; set; }
    public int Last24Hours { get; set; }
    public int Last7Days { get; set; }
    public int Last30Days { get; set; }
    public int ActiveSourceIPs { get; set; }
    public int AffectedCountries { get; set; }
    public int UniqueMalwareFamilies { get; set; }
}

/// <summary>
/// Trend indicators for different time periods
/// </summary>
public class TrendIndicatorsDto
{
    public double DailyChange { get; set; }
    public double WeeklyChange { get; set; }
    public double MonthlyChange { get; set; }
    public string TrendDirection { get; set; } = string.Empty; // "up", "down", "stable"
}

/// <summary>
/// Category-specific statistics
/// </summary>
public class CategoryStatsDto
{
    public int Count { get; set; }
    public double Percentage { get; set; }
    public double TrendChange { get; set; }
    public List<string> TopMalwareFamilies { get; set; } = new();
    public List<string> TopSourceCountries { get; set; } = new();
}

/// <summary>
/// Critical security alerts
/// </summary>
public class CriticalAlertDto
{
    public string Type { get; set; } = string.Empty; // "spike_detected", "new_malware", "geo_anomaly"
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Severity { get; set; } = string.Empty; // "critical", "high", "medium"
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Top threats aggregated data
/// </summary>
public class TopThreatsDto
{
    public List<TopItemDto> MalwareFamilies { get; set; } = new();
    public List<TopItemDto> SourceCountries { get; set; } = new();
    public List<TopItemDto> TargetedCountries { get; set; } = new();
    public List<TopItemDto> SourceIPs { get; set; } = new();
    public List<TopItemDto> TargetPorts { get; set; } = new();
}

/// <summary>
/// Generic top item structure
/// </summary>
public class TopItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty; // For flexible display (IP, port number, etc.)
    public int Count { get; set; }
    public double Percentage { get; set; }
    public double TrendChange { get; set; }
}
