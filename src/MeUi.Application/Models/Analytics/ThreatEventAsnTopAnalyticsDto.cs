namespace MeUi.Application.Models.Analytics;

/// <summary>
/// Top ASN analytics response wrapper.
/// </summary>
public class ThreatEventAsnTopAnalyticsDto
{
    public List<AsnDistributionItemDto> Asns { get; set; } = new();
    public int TotalEvents { get; set; }
    public int TotalAsns => Asns.Count;
    public TimeRangeDto TimeRange { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

public class AsnDistributionItemDto
{
    public Guid AsnRegistryId { get; set; }
    public string AsnNumber { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public List<string> TopCategories { get; set; } = new();
    public List<string> TopSourceIps { get; set; } = new();
    public double AverageRiskScore { get; set; }
}
