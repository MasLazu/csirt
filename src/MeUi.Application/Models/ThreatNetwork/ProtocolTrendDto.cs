using System;

namespace MeUi.Application.Models.ThreatNetwork;

public class ProtocolTrendDto
{
    public DateTime Time { get; set; }
    public string Protocol { get; set; } = string.Empty;
    public int Events { get; set; }
}
