namespace MeUi.Domain.Entities;

public class Protocol : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<ThreatEvent> ThreatEvents { get; set; } = [];
}