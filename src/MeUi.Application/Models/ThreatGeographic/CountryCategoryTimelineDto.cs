using System;

namespace MeUi.Application.Models.ThreatGeographic;

public class CountryCategoryTimelineDto
{
    public DateTime Time { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Events { get; set; }
}