using System;

namespace MeUi.Application.Models.ThreatNetwork;

public class CriticalPortTimePointDto
{
    public DateTime Time { get; set; }
    public string Port { get; set; } = string.Empty;
    public int Attacks { get; set; }
}
