using System;

namespace MeUi.Application.Models.ThreatTemporal;

public class TimePeriodSeriesDto
{
    public DateTime Time { get; set; }
    public string TimePeriod { get; set; } = string.Empty;
    public int Events { get; set; }
}
