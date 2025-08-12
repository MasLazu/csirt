using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.PutUserRoles;

public record PutUserRolesCommand : IRequest<IEnumerable<Guid>>
{
    public Guid UserId { get; set; }
    public IEnumerable<Guid> RoleIds { get; set; } = [];
}
