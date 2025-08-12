using MediatR;
using MeUi.Application.Commands;

namespace MeUi.Application.Features.TenantAuthorization.Commands.AssignTenantRolePermissions;

public record AssignTenantRolePermissionsCommand : BaseTenantCommand, IRequest<IEnumerable<Guid>>
{
    public Guid RoleId { get; set; }
    public IEnumerable<Guid> PermissionIds { get; set; } = [];
}