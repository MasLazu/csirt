namespace MeUi.Api.Models;

public class AccessTokenResponseData
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}