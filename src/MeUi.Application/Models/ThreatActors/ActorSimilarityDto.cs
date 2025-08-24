namespace MeUi.Application.Models.ThreatActors;

public class ActorSimilarityDto
{
    public string Actor1 { get; set; } = string.Empty;
    public string Actor2 { get; set; } = string.Empty;
    public int A1AttackTypes { get; set; }
    public int A2AttackTypes { get; set; }
    public int A1Ports { get; set; }
    public int A2Ports { get; set; }
    public int A1Malware { get; set; }
    public int A2Malware { get; set; }
    public int CommonAttackTypes { get; set; }
    public int CommonPorts { get; set; }
    public int CommonMalware { get; set; }
    public int SimilarityScore { get; set; }
}
