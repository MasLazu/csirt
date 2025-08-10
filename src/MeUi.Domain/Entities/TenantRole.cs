namespace MeUi.Domain.Entities;

public class TenantRole : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Tenant? Tenant { get; set; }
    public ICollection<TenantRolePermission> TenantRolePermissions { get; set; } = [];
    public ICollection<TenantUserRole> TenantUserRoles { get; set; } = [];
}