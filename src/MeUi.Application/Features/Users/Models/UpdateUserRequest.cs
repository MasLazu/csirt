namespace MeUi.Application.Features.Users.Models;

public record UpdateUserRequest
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? Email { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsSuspended { get; init; }
}