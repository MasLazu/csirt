using System;

namespace MeUi.Application.Models.ThreatNetwork;

public class TargetedInfrastructureDto
{
    public string TargetIp { get; set; } = string.Empty;
    public int AttacksReceived { get; set; }
    public int UniqueAttackers { get; set; }
    public int PortsAttacked { get; set; }
    public int AttackTypes { get; set; }
    public DateTime FirstAttack { get; set; }
    public DateTime LastAttack { get; set; }
}
