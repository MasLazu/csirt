namespace MeUi.Application.Models;

public class TenantRoleDto : BaseDto
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<TenantRolePermissionDto> TenantRolePermissions { get; set; } = [];
}