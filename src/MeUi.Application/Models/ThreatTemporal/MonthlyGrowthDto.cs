using System;

namespace MeUi.Application.Models.ThreatTemporal;

public class MonthlyGrowthDto
{
    public string Month { get; set; } = string.Empty;
    public string AttackCategory { get; set; } = string.Empty;
    public int Events { get; set; }
    public int PreviousMonth { get; set; }
    public decimal GrowthRate { get; set; }
}
