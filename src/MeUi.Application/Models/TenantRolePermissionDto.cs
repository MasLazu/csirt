namespace MeUi.Application.Models;

public class TenantRolePermissionDto : BaseDto
{
    public Guid TenantPermissionId { get; set; }
    public Guid TenantRoleId { get; set; }

    public TenantPermissionDto? TenantPermission { get; set; }
}