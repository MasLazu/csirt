namespace MeUi.Application.Models.Analytics;

public class ThreatEventDashboardMetricsDto
{
    public int EventsLast24Hours { get; set; }
    public int EventsLastHour { get; set; }
    public double PercentageChangeFromYesterday { get; set; }
    public int ActiveThreatsCurrently { get; set; }
    public int CriticalAlertsCount { get; set; }
    public string TopThreatCategory { get; set; } = string.Empty;
    public string TopSourceCountry { get; set; } = string.Empty;
    public List<RecentHighRiskEventDto> RecentHighRiskEvents { get; set; } = new();
}

public class RecentHighRiskEventDto
{
    public Guid EventId { get; set; }
    public DateTime Timestamp { get; set; }
    public string SourceAddress { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string MalwareFamilyName { get; set; } = string.Empty;
    public double RiskScore { get; set; }
    public string CountryName { get; set; } = string.Empty;
}
