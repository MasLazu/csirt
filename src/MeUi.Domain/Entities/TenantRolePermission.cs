namespace MeUi.Domain.Entities;

public class TenantRolePermission : BaseEntity
{
    public Guid TenantPermissionId { get; set; }
    public Guid TenantRoleId { get; set; }

    public TenantPermission? TenantPermission { get; set; }
    public TenantRole? TenantRole { get; set; }
}