namespace MeUi.Application.Models.ThreatActors;

public class ActorEvolutionDto
{
    public string ActorIp { get; set; } = string.Empty;
    public int ActiveDays { get; set; }
    public decimal AvgEventsPerDay { get; set; }
    public int PeakEventsPerDay { get; set; }
    public decimal AvgPortsPerDay { get; set; }
    public decimal AvgCategoriesPerDay { get; set; }
    public decimal EventVariance { get; set; }
}
