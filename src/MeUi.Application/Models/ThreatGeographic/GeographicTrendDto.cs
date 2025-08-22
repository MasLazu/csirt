using System;

namespace MeUi.Application.Models.ThreatGeographic;

public class GeographicTrendDto
{
    public DateTime Time { get; set; }
    public string Country { get; set; } = string.Empty;
    public int Events { get; set; }
}