namespace MeUi.Application.Models.ThreatCompliance
{
    public class RegionalRiskDto
    {
        public string TopRiskCountry { get; set; } = string.Empty;
        public int ThreatEvents { get; set; }
        public int UniqueSources { get; set; }
        public int AttackTypes { get; set; }
        public decimal RiskScore { get; set; }
    }
}
