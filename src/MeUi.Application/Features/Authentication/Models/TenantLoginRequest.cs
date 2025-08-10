namespace MeUi.Application.Features.Authentication.Models;

public record TenantLoginRequest
{
    public string EmailOrUsername { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public Guid? TenantId { get; init; }
}