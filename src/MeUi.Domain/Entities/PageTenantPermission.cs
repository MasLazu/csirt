namespace MeUi.Domain.Entities;

public class PageTenantPermission : BaseEntity
{
    public Guid PageId { get; set; }
    public Guid TenantPermissionId { get; set; }

    public Page? Page { get; set; }
    public TenantPermission? TenantPermission { get; set; }
}