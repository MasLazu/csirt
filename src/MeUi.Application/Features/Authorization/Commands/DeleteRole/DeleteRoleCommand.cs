using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.DeleteRole;

public record DeleteRoleCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
}