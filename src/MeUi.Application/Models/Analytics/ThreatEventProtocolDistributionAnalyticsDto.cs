namespace MeUi.Application.Models.Analytics;

/// <summary>
/// Protocol distribution analytics wrapper
/// </summary>
public class ThreatEventProtocolDistributionAnalyticsDto
{
    public List<ProtocolDistributionItemDto> Protocols { get; set; } = new();
    public int TotalProtocols => Protocols.Count;
    public int TotalEvents { get; set; }
    public DateTime GeneratedAt { get; set; }
    public TimeRangeDto TimeRange { get; set; } = new();
}

public class ProtocolDistributionItemDto
{
    public Guid ProtocolId { get; set; }
    public string ProtocolName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public List<int> TopPorts { get; set; } = new();
    public List<string> TopCategories { get; set; } = new();
}
