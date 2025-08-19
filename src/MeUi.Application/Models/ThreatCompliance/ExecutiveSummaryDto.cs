namespace MeUi.Application.Models.ThreatCompliance
{
    public class ExecutiveSummaryDto
    {
        public string Period { get; set; } = string.Empty;
        public int TotalThreats { get; set; }
        public int UniqueSources { get; set; }
        public int CountriesAffected { get; set; }
        public int AttackCategories { get; set; }
        public int PortsTargeted { get; set; }
        public decimal AvgSeverity { get; set; }
        public decimal GrowthRate { get; set; }
        public string ThreatLevel { get; set; } = string.Empty;
    }
}
