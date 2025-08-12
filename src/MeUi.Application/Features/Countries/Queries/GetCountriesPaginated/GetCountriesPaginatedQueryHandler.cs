using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.Countries.Queries.GetCountriesPaginated;

public class GetCountriesPaginatedQueryHandler : IRequestHandler<GetCountriesPaginatedQuery, PaginatedDto<CountryDto>>
{
    private readonly IRepository<Country> _countryRepository;

    public GetCountriesPaginatedQueryHandler(IRepository<Country> countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<PaginatedDto<CountryDto>> Handle(GetCountriesPaginatedQuery request, CancellationToken ct)
    {
        // Build search filter
        Expression<Func<Country, bool>> predicate = country => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string searchTerm = request.Search.ToLower();
            predicate = country => country.Code.ToLower().Contains(searchTerm) ||
                                  country.Name.ToLower().Contains(searchTerm);
        }

        // Build orderBy expression
        Expression<Func<Country, object>> orderBy = GetOrderByExpression(request.SortBy);

        // Get paginated countries using database-level filtering and pagination
        (IEnumerable<Country> countries, int totalItems) = await _countryRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: request.IsDescending,
            skip: (request.ValidatedPage - 1) * request.ValidatedPageSize,
            take: request.ValidatedPageSize,
            ct: ct
        );

        IEnumerable<CountryDto> countryDtos = countries.Adapt<IEnumerable<CountryDto>>();

        return new PaginatedDto<CountryDto>
        {
            Items = countryDtos,
            TotalItems = totalItems,
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize,
        };
    }

    private static Expression<Func<Country, object>> GetOrderByExpression(string? sortBy)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "code" => country => country.Code,
            "name" => country => country.Name,
            "createdat" => country => country.CreatedAt,
            "updatedat" => country => country.UpdatedAt ?? DateTime.MinValue,
            _ => country => country.Name // Default sort by Name
        };
    }
}
