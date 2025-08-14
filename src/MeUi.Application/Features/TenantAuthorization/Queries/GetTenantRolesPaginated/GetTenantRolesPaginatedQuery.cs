using MeUi.Application.Commands;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantRolesPaginated;

public record GetTenantRolesPaginatedQuery : BasePaginatedQuery<RoleDto>, ITenantRequest
{
    public Guid TenantId { get; set; }
}
