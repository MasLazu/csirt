namespace MeUi.Domain.Entities;

public class TenantUserRefreshToken : BaseEntity
{
    public Guid TenantUserId { get; set; }
    public Guid RefreshTokenId { get; set; }

    public TenantUser? TenantUser { get; set; }
    public RefreshToken? RefreshToken { get; set; }
}