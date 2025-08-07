using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetRolePermissions;

public record GetRolePermissionsQuery(Guid RoleId) : IRequest<IEnumerable<PermissionDto>>;