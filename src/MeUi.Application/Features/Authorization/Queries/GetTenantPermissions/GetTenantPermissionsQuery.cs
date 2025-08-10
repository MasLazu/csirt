using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantPermissions;

public record GetTenantPermissionsQuery : IRequest<IEnumerable<PermissionDto>>;