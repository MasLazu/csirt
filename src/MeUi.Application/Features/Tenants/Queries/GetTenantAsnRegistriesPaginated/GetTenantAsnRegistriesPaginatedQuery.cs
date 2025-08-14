using MeUi.Application.Models;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantAsnRegistriesPaginated;

public record GetTenantAsnRegistriesPaginatedQuery : BasePaginatedQuery<AsnRegistryDto>, MeUi.Application.Models.ITenantRequest
{
    public Guid TenantId { get; set; }

    public GetTenantAsnRegistriesPaginatedQuery()
    {
        SortBy = "Number"; // Default sort by ASN number
    }
}
