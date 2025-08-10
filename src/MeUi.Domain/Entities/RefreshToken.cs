namespace MeUi.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = [];
    public ICollection<TenantUserRefreshToken> TenantUserRefreshTokens { get; set; } = [];
}