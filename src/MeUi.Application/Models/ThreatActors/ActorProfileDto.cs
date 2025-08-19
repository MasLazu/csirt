using System;

namespace MeUi.Application.Models.ThreatActors
{
    public class ActorProfileDto
    {
        public string ActorIp { get; set; } = string.Empty;
        public string OriginCountry { get; set; } = string.Empty;
        public string Asn { get; set; } = string.Empty;
        public int TotalAttacks { get; set; }
        public int PortsTargeted { get; set; }
        public int AttackTypes { get; set; }
        public int UniqueTargets { get; set; }
        public int MalwareUsed { get; set; }
        public decimal CampaignDurationDays { get; set; }
        public decimal ActorThreatScore { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
