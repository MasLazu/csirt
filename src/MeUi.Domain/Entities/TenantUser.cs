namespace MeUi.Domain.Entities;

public class TenantUser : BaseEntity
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSuspended { get; set; } = false;
    public Guid TenantId { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public ICollection<TenantUserLoginMethod> TenantUserLoginMethods { get; set; } = [];
    public ICollection<TenantUserRefreshToken> TenantUserRefreshTokens { get; set; } = [];
    public ICollection<TenantUserRole> TenantUserRoles { get; set; } = [];
    public ICollection<TenantUserPassword> TenantUserPasswords { get; set; } = [];
}