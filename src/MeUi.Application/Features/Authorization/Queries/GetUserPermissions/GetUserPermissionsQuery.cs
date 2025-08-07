using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetUserPermissions;

public class GetUserPermissionsQuery : IRequest<IEnumerable<PermissionDto>>
{
    public Guid UserId { get; set; }
}