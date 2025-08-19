namespace MeUi.Application.Models.ThreatNetwork;

public class AsnNetworkDto
{
    public string ASN { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public int Events { get; set; }
    public int UniqueIps { get; set; }
    public int PortsTargeted { get; set; }
    public int AttackTypes { get; set; }
}
