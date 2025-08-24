namespace MeUi.Api.Models;

public class TenantAccessTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public Guid TenantId { get; set; }
}