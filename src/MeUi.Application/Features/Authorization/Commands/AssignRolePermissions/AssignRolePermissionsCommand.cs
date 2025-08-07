using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.AssignRolePermissions;

public record AssignRolePermissionsCommand : IRequest<IEnumerable<Guid>>
{
    public Guid RoleId { get; init; }
    public IEnumerable<Guid> PermissionIds { get; init; } = [];
}