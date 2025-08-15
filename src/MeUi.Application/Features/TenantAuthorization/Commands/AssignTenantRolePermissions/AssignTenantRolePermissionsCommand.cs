using MediatR;
using MeUi.Application.Commands;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Commands.AssignTenantRolePermissions;

public record AssignTenantRolePermissionsCommand : BaseTenantCommand, IRequest<IEnumerable<Guid>>, ITenantRequest
{
    public Guid RoleId { get; set; }
    public IEnumerable<Guid> PermissionIds { get; set; } = [];
}