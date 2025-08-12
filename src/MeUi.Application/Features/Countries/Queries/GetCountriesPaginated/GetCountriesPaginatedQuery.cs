using MeUi.Application.Models;

namespace MeUi.Application.Features.Countries.Queries.GetCountriesPaginated;

public record GetCountriesPaginatedQuery : BasePaginatedQuery<CountryDto>
{
    public GetCountriesPaginatedQuery()
    {
        SortBy = "Name"; // Default sort by country name
    }
}
