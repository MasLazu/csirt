using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantPermissions;

public record GetTenantPermissionsQuery : IRequest<IEnumerable<PermissionDto>>;