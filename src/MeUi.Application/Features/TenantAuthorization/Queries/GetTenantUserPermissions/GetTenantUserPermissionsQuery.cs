using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantUserPermissions;

public record GetTenantUserPermissionsQuery : IRequest<IEnumerable<PermissionDto>>
{
    public Guid UserId { get; set; }
};