namespace MeUi.Application.Models.ThreatNetwork;

public class TargetedPortDto
{
    public string Port { get; set; } = string.Empty;
    public int Attacks { get; set; }
    public int UniqueSources { get; set; }
}
