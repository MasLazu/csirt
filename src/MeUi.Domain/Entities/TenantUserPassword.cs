namespace MeUi.Domain.Entities;

public class TenantUserPassword : BaseEntity
{
    public Guid PasswordId { get; set; }
    public Guid TenantUserLoginMethodId { get; set; }

    public Password? Password { get; set; }
    public TenantUserLoginMethod? TenantUserLoginMethod { get; set; }
}