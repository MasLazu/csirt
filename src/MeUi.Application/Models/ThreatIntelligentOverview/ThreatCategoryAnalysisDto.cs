namespace MeUi.Application.Models.ThreatIntelligentOverview;

public class ThreatCategoryAnalysisDto
{
    public string Category { get; set; } = string.Empty;
    public int TotalEvents { get; set; }
    public int UniqueIps { get; set; }
    public int Countries { get; set; }
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
}
