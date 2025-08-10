namespace MeUi.Domain.Entities;

public class TenantAsnRegistry : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid AsnRegistryId { get; set; }

    public Tenant? Tenant { get; set; }
    public AsnRegistry? AsnRegistry { get; set; }
}