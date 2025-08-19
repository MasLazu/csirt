namespace MeUi.Application.Models.ThreatActors
{
    public class ActorTtpDto
    {
        public string ActorIp { get; set; } = string.Empty;
        public string TtpProfile { get; set; } = string.Empty;
        public int TtpDiversity { get; set; }
        public int TotalEvents { get; set; }
    }
}
