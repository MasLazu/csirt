using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantUserPermissions;

public record GetTenantUserPermissionsQuery(Guid UserId) : IRequest<IEnumerable<PermissionDto>>;