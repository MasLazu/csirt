namespace MeUi.Domain.Entities;

public class AsnRegistry : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<ThreatEvent> ThreatEvents { get; set; } = [];
}