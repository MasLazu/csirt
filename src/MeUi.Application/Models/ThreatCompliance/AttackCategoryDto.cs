namespace MeUi.Application.Models.ThreatCompliance;

public class AttackCategoryDto
{
    public string AttackCategory { get; set; } = string.Empty;
    public int TotalEvents { get; set; }
    public int UniqueSources { get; set; }
    public int Countries { get; set; }
    public decimal AvgAgeHours { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;
}
