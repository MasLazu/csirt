namespace MeUi.Application.Models;

public class PageTenantPermissionDto : BaseDto
{
    public Guid PageId { get; set; }
    public Guid TenantPermissionId { get; set; }

    public PageDto? Page { get; set; }
    public TenantPermissionDto? TenantPermission { get; set; }
}