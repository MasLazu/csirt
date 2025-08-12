using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.AssignUserRoles;

public record AssignUserRolesCommand : IRequest<IEnumerable<Guid>>
{
    public Guid UserId { get; set; }
    public IEnumerable<Guid> RoleIds { get; set; } = [];
}