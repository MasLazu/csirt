using MediatR;

namespace MeUi.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
}