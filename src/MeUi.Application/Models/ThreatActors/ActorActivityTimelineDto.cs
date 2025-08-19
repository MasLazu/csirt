using System;

namespace MeUi.Application.Models.ThreatActors
{
    public class ActorActivityTimelineDto
    {
        public DateTime Time { get; set; }
        public string ActorIp { get; set; } = string.Empty;
        public int Activity { get; set; }
    }
}
