namespace MeUi.Application.Models;

public class TenantAsnRegistryDto : BaseDto
{
    public Guid TenantId { get; set; }
    public Guid AsnRegistryId { get; set; }

    public AsnRegistryDto? AsnRegistry { get; set; }
}