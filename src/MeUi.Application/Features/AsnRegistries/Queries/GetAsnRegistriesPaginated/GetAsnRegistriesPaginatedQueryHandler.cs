using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.AsnRegistries.Queries.GetAsnRegistriesPaginated;

public class GetAsnRegistriesPaginatedQueryHandler : IRequestHandler<GetAsnRegistriesPaginatedQuery, PaginatedDto<AsnRegistryDto>>
{
    private readonly IRepository<AsnRegistry> _asnRegistryRepository;

    public GetAsnRegistriesPaginatedQueryHandler(IRepository<AsnRegistry> asnRegistryRepository)
    {
        _asnRegistryRepository = asnRegistryRepository;
    }

    public async Task<PaginatedDto<AsnRegistryDto>> Handle(GetAsnRegistriesPaginatedQuery request, CancellationToken ct)
    {
        // Build search filter
        Expression<Func<AsnRegistry, bool>> predicate = asn => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string searchTerm = request.Search.ToLower();
            predicate = asn => asn.Number.ToLower().Contains(searchTerm) ||
                              asn.Description.ToLower().Contains(searchTerm);
        }

        // Build orderBy expression
        Expression<Func<AsnRegistry, object>> orderBy = GetOrderByExpression(request.SortBy);

        // Get paginated ASN registries using database-level filtering and pagination
        (IEnumerable<AsnRegistry> asnRegistries, int totalItems) = await _asnRegistryRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: request.IsDescending,
            skip: (request.ValidatedPage - 1) * request.ValidatedPageSize,
            take: request.ValidatedPageSize,
            ct: ct
        );

        IEnumerable<AsnRegistryDto> asnRegistryDtos = asnRegistries.Adapt<IEnumerable<AsnRegistryDto>>();

        return new PaginatedDto<AsnRegistryDto>
        {
            Items = asnRegistryDtos,
            TotalItems = totalItems,
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize,
        };
    }

    private static Expression<Func<AsnRegistry, object>> GetOrderByExpression(string? sortBy)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "number" => asn => asn.Number,
            "description" => asn => asn.Description,
            "createdat" => asn => asn.CreatedAt,
            "updatedat" => asn => asn.UpdatedAt ?? DateTime.MinValue,
            _ => asn => asn.Number // Default sort by Number
        };
    }
}