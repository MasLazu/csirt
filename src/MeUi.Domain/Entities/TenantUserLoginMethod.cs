namespace MeUi.Domain.Entities;

public class TenantUserLoginMethod : BaseEntity
{
    public Guid TenantUserId { get; set; }
    public string LoginMethodCode { get; set; } = string.Empty;

    public TenantUser? TenantUser { get; set; }
    public LoginMethod? LoginMethod { get; set; }
}