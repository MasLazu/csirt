namespace MeUi.Application.Models;

public class TenantRoleDto : BaseDto
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public IEnumerable<PermissionDto> Permissions { get; set; } = [];
    public IEnumerable<PageGroupDto> AccessiblePageGroups { get; set; } = [];
}