using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantAsnRegistriesPaginated;

public class GetTenantAsnRegistriesPaginatedQueryHandler : IRequestHandler<GetTenantAsnRegistriesPaginatedQuery, PaginatedDto<AsnRegistryDto>>
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IRepository<TenantAsnRegistry> _tenantAsnRegistryRepository;
    private readonly IRepository<AsnRegistry> _asnRegistryRepository;

    public GetTenantAsnRegistriesPaginatedQueryHandler(
        IRepository<Tenant> tenantRepository,
        IRepository<TenantAsnRegistry> tenantAsnRegistryRepository,
        IRepository<AsnRegistry> asnRegistryRepository)
    {
        _tenantRepository = tenantRepository;
        _tenantAsnRegistryRepository = tenantAsnRegistryRepository;
        _asnRegistryRepository = asnRegistryRepository;
    }

    public async Task<PaginatedDto<AsnRegistryDto>> Handle(GetTenantAsnRegistriesPaginatedQuery request, CancellationToken ct)
    {
        // Verify tenant exists
        if (!await _tenantRepository.ExistsAsync(request.TenantId, ct))
        {
            throw new NotFoundException($"Tenant with ID {request.TenantId} not found");
        }

        // Get ASN registry IDs assigned to this tenant
        IEnumerable<Guid> tenantAsnRegistryIds = (await _tenantAsnRegistryRepository
            .FindAsync(tar => tar.TenantId == request.TenantId, ct))
            .Select(tar => tar.AsnRegistryId)
            .ToList();

        // Build search filter for ASN registries assigned to this tenant
        Expression<Func<AsnRegistry, bool>> predicate = asn => tenantAsnRegistryIds.Contains(asn.Id);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string searchTerm = request.Search.ToLower();
            predicate = asn => tenantAsnRegistryIds.Contains(asn.Id) &&
                              (asn.Number.ToLower().Contains(searchTerm) ||
                               asn.Description.ToLower().Contains(searchTerm));
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