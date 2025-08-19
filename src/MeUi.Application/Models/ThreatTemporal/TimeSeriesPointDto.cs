using System;

namespace MeUi.Application.Models.ThreatTemporal;

public class TimeSeriesPointDto
{
    public DateTime Time { get; set; }
    public int Events { get; set; }
}
