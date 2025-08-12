using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetMyTenantUserRoles;

public record GetMyTenantUserRolesQuery : IRequest<IEnumerable<TenantRoleDto>>
{
    public Guid TenantUserId { get; set; }
}
