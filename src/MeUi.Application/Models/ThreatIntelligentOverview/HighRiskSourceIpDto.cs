namespace MeUi.Application.Models.ThreatIntelligentOverview
{
    public class HighRiskSourceIpDto
    {
        public string SourceIp { get; set; } = string.Empty;
        public int Events { get; set; }
        public int Categories { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
