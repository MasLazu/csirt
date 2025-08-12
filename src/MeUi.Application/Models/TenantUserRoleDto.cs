namespace MeUi.Application.Models;

public class TenantUserRoleDto : BaseDto
{
    public Guid TenantUserId { get; set; }
    public Guid TenantRoleId { get; set; }

    public TenantRoleDto? TenantRole { get; set; }
}