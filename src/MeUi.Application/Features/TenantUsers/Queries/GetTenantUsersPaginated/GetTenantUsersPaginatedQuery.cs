using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUsersPaginated;

public record GetTenantUsersPaginatedQuery : BasePaginatedQuery<TenantUserDto>
{
    public Guid TenantId { get; set; }
    public bool? IsSuspended { get; set; }
}