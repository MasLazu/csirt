namespace MeUi.Application.Models.ThreatCompliance
{
    public class KpiTrendPointDto
    {
        public System.DateTime Time { get; set; }
        public string Metric { get; set; } = string.Empty;
        public long Value { get; set; }
    }
}
