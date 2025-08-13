using System.Net;
using MeUi.Domain.Entities;

namespace MeUi.Application.Interfaces;

/// <summary>
/// Specialized analytics repository for ThreatEvent entities
/// Optimized for TimescaleDB hypertable operations and threat intelligence analytics
/// </summary>
public interface IThreatEventAnalyticsRepository
{
    #region Timeline Analytics

    /// <summary>
    /// Get threat event timeline data with time-bucketed aggregations
    /// Optimized for TimescaleDB time_bucket functions
    /// </summary>
    Task<IEnumerable<TimelineDataPoint>> GetTimelineAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        string timeInterval, // "hour", "day", "week", "month"
        Guid? tenantId = null,
        string? category = null,
        Guid? malwareFamilyId = null,
        Guid? sourceCountryId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get comparative timeline data between two time periods
    /// </summary>
    Task<IEnumerable<ComparativeTimelineDataPoint>> GetComparativeTimelineAsync(
        DateTime currentStartTime,
        DateTime currentEndTime,
        DateTime comparisonStartTime,
        DateTime comparisonEndTime,
        string timeInterval,
        Guid? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get malware family counts per time bucket (parallel to category timeline)
    /// </summary>
    Task<IEnumerable<MalwareTimelineDataPoint>> GetMalwareTimelineAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        string timeInterval,
        Guid? tenantId = null,
        CancellationToken ct = default);

    #endregion

    #region Summary Analytics

    /// <summary>
    /// Get comprehensive threat event summary statistics
    /// </summary>
    Task<ThreatEventSummary> GetSummaryAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        Guid? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get real-time dashboard metrics
    /// </summary>
    Task<ThreatEventDashboardMetrics> GetDashboardMetricsAsync(
        Guid? tenantId = null,
        CancellationToken ct = default);

    #endregion

    #region Categorical Analytics

    /// <summary>
    /// Get top threat categories with counts and trends
    /// </summary>
    Task<IEnumerable<CategoryAnalytics>> GetTopCategoriesAsync(
        DateTime startTime,
        DateTime endTime,
        int topCount = 10,
        Guid? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get malware family distribution and trends
    /// </summary>
    Task<IEnumerable<MalwareFamilyAnalytics>> GetMalwareFamilyAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        int topCount = 10,
        Guid? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get geographical distribution of threat sources
    /// </summary>
    Task<IEnumerable<GeographicalAnalytics>> GetGeographicalAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        int topCount = 20,
        Guid? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get ASN-based threat analytics
    /// </summary>
    Task<IEnumerable<AsnAnalytics>> GetAsnAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        int topCount = 15,
        Guid? tenantId = null,
        CancellationToken ct = default);

    #endregion

    #region Network Analytics

    /// <summary>
    /// Get port-based threat analytics
    /// </summary>
    Task<IEnumerable<PortAnalytics>> GetPortAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        bool isSourcePort = true,
        int topCount = 20,
        Guid? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get network protocol distribution
    /// </summary>
    Task<IEnumerable<ProtocolAnalytics>> GetProtocolAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        Guid? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get IP address reputation analytics
    /// </summary>
    Task<IEnumerable<IpReputationAnalytics>> GetIpReputationAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        int topCount = 50,
        bool isSourceIp = true,
        Guid? tenantId = null,
        CancellationToken ct = default);

    #endregion

    #region Advanced Analytics

    /// <summary>
    /// Get threat event correlation patterns
    /// </summary>
    Task<IEnumerable<CorrelationPattern>> GetCorrelationPatternsAsync(
        DateTime startTime,
        DateTime endTime,
        string correlationType, // "category_malware", "geo_port", "asn_category"
        Guid? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get anomaly detection results for threat volumes
    /// </summary>
    Task<IEnumerable<AnomalyDetectionResult>> GetAnomalyDetectionAsync(
        DateTime startTime,
        DateTime endTime,
        string anomalyType, // "volume", "geographic", "temporal"
        double sensitivityThreshold = 2.0,
        Guid? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get threat intelligence attribution data
    /// </summary>
    Task<IEnumerable<AttributionAnalytics>> GetAttributionAnalyticsAsync(
        DateTime startTime,
        DateTime endTime,
        Guid? tenantId = null,
        CancellationToken ct = default);

    #endregion

    #region Batch Operations

    /// <summary>
    /// Execute multiple analytics queries in a single batch for dashboard optimization
    /// </summary>
    Task<ThreatEventBatchAnalyticsResult> ExecuteBatchAnalyticsAsync(
        IEnumerable<ThreatEventAnalyticsQuery> queries,
        Guid? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Get comprehensive threat landscape overview
    /// </summary>
    Task<ThreatLandscapeOverview> GetThreatLandscapeOverviewAsync(
        DateTime startTime,
        DateTime endTime,
        Guid? tenantId = null,
        CancellationToken ct = default);

    #endregion
}

#region Analytics Result Models

public class TimelineDataPoint
{
    public DateTime Timestamp { get; set; }
    public int Count { get; set; }
    public string Category { get; set; } = string.Empty;
    public double? AverageRiskScore { get; set; }
    public int UniqueSourceIps { get; set; }
    public int UniqueDestinationIps { get; set; }
}

public class ComparativeTimelineDataPoint : TimelineDataPoint
{
    public int PreviousPeriodCount { get; set; }
    public double PercentageChange { get; set; }
    public string TrendDirection { get; set; } = string.Empty;
}

public class MalwareTimelineDataPoint
{
    public DateTime Timestamp { get; set; }
    public string MalwareFamilyName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ThreatEventSummary
{
    public int TotalEvents { get; set; }
    public int UniqueSourceIps { get; set; }
    public int UniqueDestinationIps { get; set; }
    public int UniqueMalwareFamilies { get; set; }
    public int UniqueCountries { get; set; }
    public int UniqueAsns { get; set; }
    public string MostActiveCategory { get; set; } = string.Empty;
    public string MostTargetedCountry { get; set; } = string.Empty;
    public string MostActiveMalwareFamily { get; set; } = string.Empty;
    public double AverageEventsPerHour { get; set; }
    public DateTime PeakActivityTime { get; set; }
    public int PeakActivityCount { get; set; }
    public Dictionary<string, int> CategoryDistribution { get; set; } = new();
}

public class ThreatEventDashboardMetrics
{
    public int EventsLast24Hours { get; set; }
    public int EventsLastHour { get; set; }
    public double PercentageChangeFromYesterday { get; set; }
    public int ActiveThreatsCurrently { get; set; }
    public int CriticalAlertsCount { get; set; }
    public string TopThreatCategory { get; set; } = string.Empty;
    public string TopSourceCountry { get; set; } = string.Empty;
    public List<RecentHighRiskEvent> RecentHighRiskEvents { get; set; } = new();
}

public class CategoryAnalytics
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public double PercentageChange { get; set; }
    public string TrendDirection { get; set; } = string.Empty;
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
}

public class MalwareFamilyAnalytics
{
    public Guid MalwareFamilyId { get; set; }
    public string FamilyName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public double RiskScore { get; set; }
    public List<string> AssociatedCategories { get; set; } = new();
    public List<string> TopSourceCountries { get; set; } = new();
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
}

public class GeographicalAnalytics
{
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public bool IsSource { get; set; }
    public List<string> TopCategories { get; set; } = new();
    public List<string> TopMalwareFamilies { get; set; } = new();
    public double AverageRiskScore { get; set; }
}

public class AsnAnalytics
{
    public Guid AsnRegistryId { get; set; }
    public string AsnNumber { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public List<string> TopCategories { get; set; } = new();
    public List<IPAddress> TopSourceIps { get; set; } = new();
    public double AverageRiskScore { get; set; }
}

public class PortAnalytics
{
    public int Port { get; set; }
    public string PortType { get; set; } = string.Empty; // "source" or "destination"
    public int Count { get; set; }
    public double Percentage { get; set; }
    public List<string> AssociatedServices { get; set; } = new();
    public List<string> TopCategories { get; set; } = new();
    public double RiskScore { get; set; }
}

public class ProtocolAnalytics
{
    public Guid ProtocolId { get; set; }
    public string ProtocolName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public List<int> TopPorts { get; set; } = new();
    public List<string> TopCategories { get; set; } = new();
}

public class IpReputationAnalytics
{
    public IPAddress IpAddress { get; set; } = IPAddress.None;
    public string IpType { get; set; } = string.Empty; // "source" or "destination"
    public int Count { get; set; }
    public double RiskScore { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string AsnOrganization { get; set; } = string.Empty;
    public List<string> AssociatedCategories { get; set; } = new();
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
}

public class CorrelationPattern
{
    public string PatternType { get; set; } = string.Empty;
    public string PrimaryKey { get; set; } = string.Empty;
    public string SecondaryKey { get; set; } = string.Empty;
    public int Co_OccurrenceCount { get; set; }
    public double CorrelationStrength { get; set; }
    public double ConditionalProbability { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class AnomalyDetectionResult
{
    public DateTime Timestamp { get; set; }
    public string AnomalyType { get; set; } = string.Empty;
    public double AnomalyScore { get; set; }
    public double ExpectedValue { get; set; }
    public double ActualValue { get; set; }
    public double DeviationScore { get; set; }
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
}

public class AttributionAnalytics
{
    public string AttributionSource { get; set; } = string.Empty;
    public string ThreatActorGroup { get; set; } = string.Empty;
    public List<string> AssociatedMalwareFamilies { get; set; } = new();
    public List<string> TargetedCountries { get; set; } = new();
    public List<string> AttackVectors { get; set; } = new();
    public double ConfidenceScore { get; set; }
    public DateTime FirstAttributed { get; set; }
    public DateTime LastActivity { get; set; }
}

public class RecentHighRiskEvent
{
    public Guid EventId { get; set; }
    public DateTime Timestamp { get; set; }
    public IPAddress SourceAddress { get; set; } = IPAddress.None;
    public string Category { get; set; } = string.Empty;
    public string MalwareFamilyName { get; set; } = string.Empty;
    public double RiskScore { get; set; }
    public string CountryName { get; set; } = string.Empty;
}

public class ThreatEventAnalyticsQuery
{
    public string QueryId { get; set; } = string.Empty;
    public string QueryType { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class ThreatEventBatchAnalyticsResult
{
    public Dictionary<string, object> Results { get; set; } = new();
    public TimeSpan ExecutionTime { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int QueriesExecuted { get; set; }
}

public class ThreatLandscapeOverview
{
    public ThreatEventSummary Summary { get; set; } = new();
    public IEnumerable<CategoryAnalytics> TopCategories { get; set; } = new List<CategoryAnalytics>();
    public IEnumerable<GeographicalAnalytics> TopSourceCountries { get; set; } = new List<GeographicalAnalytics>();
    public IEnumerable<MalwareFamilyAnalytics> TopMalwareFamilies { get; set; } = new List<MalwareFamilyAnalytics>();
    public IEnumerable<AsnAnalytics> TopAsns { get; set; } = new List<AsnAnalytics>();
    public IEnumerable<TimelineDataPoint> HourlyTimeline { get; set; } = new List<TimelineDataPoint>();
    public IEnumerable<AnomalyDetectionResult> RecentAnomalies { get; set; } = new List<AnomalyDetectionResult>();
    public DateTime GeneratedAt { get; set; }
    public TimeSpan AnalysisPeriod { get; set; }
}

#endregion
