namespace MeUi.Application.Models.Analytics;

/// <summary>
/// Response model for threat event timeline analytics
/// </summary>
public class ThreatEventTimelineDto
{
    public DateTime Timestamp { get; set; }
    public int EventCount { get; set; }
    public Dictionary<string, int> Categories { get; set; } = new();
    public Dictionary<string, int> Severity { get; set; } = new();
    public Dictionary<string, int> MalwareFamilies { get; set; } = new();
}

/// <summary>
/// Complete timeline analytics response
/// </summary>
public class ThreatEventTimelineAnalyticsDto
{
    public List<ThreatEventTimelineDto> Timeline { get; set; } = new();
    public int TotalEvents { get; set; }
    public TimeRangeDto TimeRange { get; set; } = new();
    public string Interval { get; set; } = string.Empty;
    public TrendAnalysisDto Trends { get; set; } = new();
}

/// <summary>
/// Time range information
/// </summary>
public class TimeRangeDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

/// <summary>
/// Trend analysis data
/// </summary>
public class TrendAnalysisDto
{
    public double PercentageChange { get; set; }
    public string Direction { get; set; } = string.Empty; // "increasing", "decreasing", "stable"
    public DateTime ComparisonPeriodStart { get; set; }
    public DateTime ComparisonPeriodEnd { get; set; }
}
