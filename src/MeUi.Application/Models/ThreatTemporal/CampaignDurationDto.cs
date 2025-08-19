using System;

namespace MeUi.Application.Models.ThreatTemporal;

public class CampaignDurationDto
{
    public string SourceIp { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int TotalEvents { get; set; }
    public decimal CampaignDurationHours { get; set; }
    public decimal EventsPerHour { get; set; }
    public DateTime CampaignStart { get; set; }
    public DateTime CampaignEnd { get; set; }
}
