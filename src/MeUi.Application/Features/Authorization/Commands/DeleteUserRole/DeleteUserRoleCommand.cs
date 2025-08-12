using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.DeleteUserRole;

public record DeleteUserRoleCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}
