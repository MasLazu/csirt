using MediatR;

namespace MeUi.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<Guid>
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}