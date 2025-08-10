namespace MeUi.Domain.Entities;

public class TenantUserRole : BaseEntity
{
    public Guid TenantUserId { get; set; }
    public Guid TenantRoleId { get; set; }

    public TenantUser? TenantUser { get; set; }
    public TenantRole? TenantRole { get; set; }
}