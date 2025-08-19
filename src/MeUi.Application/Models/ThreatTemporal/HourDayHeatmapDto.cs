using System;

namespace MeUi.Application.Models.ThreatTemporal;

public class HourDayHeatmapDto
{
    public int Hour { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public int Events { get; set; }
}
