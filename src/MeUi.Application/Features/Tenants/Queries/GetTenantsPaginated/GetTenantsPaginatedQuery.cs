using MeUi.Application.Models;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantsPaginated;

public record GetTenantsPaginatedQuery : BasePaginatedQuery<TenantDto>
{
    public bool? IsActive { get; set; }
}