namespace MeUi.Application.Features.Authentication.Models;

public record TenantUserRegistrationRequest
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public Guid TenantId { get; init; }
    public bool IsTenantAdmin { get; init; } = false;
}