namespace MeUi.Application.Features.Authentication.Models;

public record LoginRequest
{
    public string EmailOrUsername { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}