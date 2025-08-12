using MeUi.Application.Models;

namespace MeUi.Application.Features.AsnRegistries.Queries.GetAsnRegistriesPaginated;

public record GetAsnRegistriesPaginatedQuery : BasePaginatedQuery<AsnRegistryDto>
{
    public GetAsnRegistriesPaginatedQuery()
    {
        SortBy = "Number"; // Default sort by ASN number
    }
}
