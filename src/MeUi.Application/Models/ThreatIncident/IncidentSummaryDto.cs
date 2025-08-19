using System;

namespace MeUi.Application.Models.ThreatIncident;

public class IncidentSummaryDto
{
    public string SourceIP { get; set; } = string.Empty;
    public string SourceCountry { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty;
    public int EventCount { get; set; }
    public DateTime FirstDetected { get; set; }
    public DateTime LastActivity { get; set; }
    public double DurationHours { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Priority { get; set; }
}
