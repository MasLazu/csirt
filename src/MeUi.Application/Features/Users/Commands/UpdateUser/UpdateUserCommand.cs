using MediatR;
using MeUi.Application;

namespace MeUi.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSuspended { get; set; }
}