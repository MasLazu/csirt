using MeUi.Application.Models;

namespace MeUi.Application.Features.Tenants.Models;

public class AsnRegistryDto : BaseDto
{
    public string Number { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}