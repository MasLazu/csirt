namespace MeUi.Domain.Entities;

public class Country : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<ThreatEvent> SourceThreats { get; set; } = [];
    public virtual ICollection<ThreatEvent> DestinationThreats { get; set; } = [];
}