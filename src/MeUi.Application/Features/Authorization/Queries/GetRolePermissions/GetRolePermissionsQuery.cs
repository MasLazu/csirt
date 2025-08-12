using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetRolePermissions;

public record GetRolePermissionsQuery : IRequest<IEnumerable<PermissionDto>>
{
    public Guid RoleId { get; set; }
}