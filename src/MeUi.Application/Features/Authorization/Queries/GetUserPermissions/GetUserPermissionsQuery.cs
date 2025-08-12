using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetUserPermissions;

public class GetUserPermissionsQuery : IRequest<IEnumerable<PermissionDto>>
{
    public Guid UserId { get; set; }
}