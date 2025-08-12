namespace MeUi.Api.Models;

public class AccessTokenResponseData
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}