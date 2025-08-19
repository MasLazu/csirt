using System;

namespace MeUi.Application.Models.ThreatTemporal;

public class PeakActivityDto
{
    public int Hour { get; set; }
    public string AttackCategory { get; set; } = string.Empty;
    public int TotalEvents { get; set; }
    public int UniqueSources { get; set; }
    public int PortsTargeted { get; set; }
    public decimal AvgEventsPerHour { get; set; }
}
