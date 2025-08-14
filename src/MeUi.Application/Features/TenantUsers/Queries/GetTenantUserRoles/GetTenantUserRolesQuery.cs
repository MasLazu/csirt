using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUserRoles;

public record GetTenantUserRolesQuery : IRequest<IEnumerable<RoleDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
}
