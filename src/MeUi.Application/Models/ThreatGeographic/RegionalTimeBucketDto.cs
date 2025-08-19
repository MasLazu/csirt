namespace MeUi.Application.Models.ThreatGeographic;

public class RegionalTimeBucketDto
{
    public string TimePeriod { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Events { get; set; }
}
