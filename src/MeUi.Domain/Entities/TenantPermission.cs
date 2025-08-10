namespace MeUi.Domain.Entities;

public class TenantPermission : BaseEntity
{
    public string ResourceCode { get; set; } = string.Empty;
    public string ActionCode { get; set; } = string.Empty;

    public Resource? Resource { get; set; }
    public Action? Action { get; set; }
    public ICollection<TenantRolePermission> TenantRolePermissions { get; set; } = [];
    public ICollection<PageTenantPermission> PageTenantPermissions { get; set; } = [];
}