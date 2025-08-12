using MediatR;
using MeUi.Application.Commands;

namespace MeUi.Application.Features.TenantAuthorization.Commands.AssignTenantUserRoles;

public record AssignTenantUserRolesCommand : BaseTenantCommand, IRequest<IEnumerable<Guid>>
{
    public Guid UserId { get; set; }
    public IEnumerable<Guid> RoleIds { get; set; } = [];
}