using System;

namespace MeUi.Application.Models.ThreatNetwork;

public class HighRiskIpDto
{
    public string IpAddress { get; set; } = string.Empty;
    public int TotalAttacks { get; set; }
    public int PortsTargeted { get; set; }
    public int AttackTypes { get; set; }
    public int ProtocolsUsed { get; set; }
    public double AttackScore { get; set; }
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
}
