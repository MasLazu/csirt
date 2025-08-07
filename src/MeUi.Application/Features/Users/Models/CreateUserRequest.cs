namespace MeUi.Application.Features.Users.Models;

public record CreateUserRequest
{
    public string? Username { get; init; }
    public string? Email { get; init; }
    public string Name { get; init; } = string.Empty;
}