using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetSpecificUserPermissions;

public class GetSpecificUserPermissionsQuery : IRequest<IEnumerable<PermissionDto>>
{
    public Guid UserId { get; set; }
}
