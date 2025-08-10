namespace MeUi.Application.Features.ThreatIntelligence.Models;

/// <summary>
/// DTO for hourly threat count aggregation
/// </summary>
public class HourlyThreatCountDto
{
    public DateTime Hour { get; set; }
    public long ThreatCount { get; set; }
}

/// <summary>
/// DTO for daily threat count aggregation
/// </summary>
public class DailyThreatCountDto
{
    public DateTime Day { get; set; }
    public long ThreatCount { get; set; }
}

/// <summary>
/// DTO for ASN threat count aggregation
/// </summary>
public class AsnThreatCountDto
{
    public string Asn { get; set; } = string.Empty;
    public string AsnDescription { get; set; } = string.Empty;
    public long ThreatCount { get; set; }
}

/// <summary>
/// DTO for country threat count aggregation
/// </summary>
public class CountryThreatCountDto
{
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public long ThreatCount { get; set; }
}

/// <summary>
/// DTO for category threat count aggregation
/// </summary>
public class CategoryThreatCountDto
{
    public string Category { get; set; } = string.Empty;
    public long ThreatCount { get; set; }
}

/// <summary>
/// DTO for TimescaleDB chunk information
/// </summary>
public class ChunkInfoDto
{
    public string ChunkName { get; set; } = string.Empty;
    public DateTime RangeStart { get; set; }
    public DateTime RangeEnd { get; set; }
    public long TableSize { get; set; }
    public long IndexSize { get; set; }
    public long TotalSize { get; set; }
}

/// <summary>
/// DTO for time series data points (unified for both hourly and daily)
/// </summary>
public class TimeSeriesDataPointDto
{
    public DateTime Timestamp { get; set; }
    public long ThreatCount { get; set; }
    public string Interval { get; set; } = string.Empty; // "hour" or "day"
}