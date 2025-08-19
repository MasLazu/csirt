using System;

namespace MeUi.Application.Models.ThreatIncident;

public class ResponseTimeMetricDto
{
    public DateTime Time { get; set; }
    public string Metric { get; set; } = string.Empty;
    public double Hours { get; set; }
}
