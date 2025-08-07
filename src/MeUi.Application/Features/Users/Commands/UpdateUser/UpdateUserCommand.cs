using MediatR;
using MeUi.Application;

namespace MeUi.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? Email { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsSuspended { get; init; }
}