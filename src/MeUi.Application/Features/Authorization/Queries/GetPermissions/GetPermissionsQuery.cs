using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetPermissions;

public class GetPermissionsQuery : IRequest<IEnumerable<PermissionDto>>;