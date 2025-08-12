using MediatR;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using Mapster;
using System.Linq.Expressions;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantsPaginated;

public class GetTenantsPaginatedQueryHandler : IRequestHandler<GetTenantsPaginatedQuery, PaginatedDto<TenantDto>>
{
    private readonly IRepository<Tenant> _tenantRepository;

    public GetTenantsPaginatedQueryHandler(IRepository<Tenant> tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<PaginatedDto<TenantDto>> Handle(GetTenantsPaginatedQuery request, CancellationToken ct)
    {
        // Build the predicate for filtering
        Expression<Func<Tenant, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(request.Search))
        {
            predicate = t => t.Name.Contains(request.Search) ||
                            t.Description != null && t.Description.Contains(request.Search);
        }

        if (request.IsActive.HasValue)
        {
            bool isActive = request.IsActive.Value;
            predicate = predicate == null
                ? t => t.IsActive == isActive
                : t => t.IsActive == isActive &&
                       (t.Name.Contains(request.Search!) ||
                        t.Description != null && t.Description.Contains(request.Search!));
        }

        // Determine sort field and direction
        Expression<Func<Tenant, object>> orderBy = request.SortBy?.ToLower() switch
        {
            "description" => t => t.Description ?? string.Empty,
            "isactive" => t => t.IsActive,
            "createdat" => t => t.CreatedAt,
            "updatedat" => t => t.UpdatedAt,
            _ => t => t.Name // Default sort by name
        };

        (IEnumerable<Tenant> items, int totalCount) = await _tenantRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: request.IsDescending,
            skip: (request.ValidatedPage - 1) * request.ValidatedPageSize,
            take: request.ValidatedPageSize,
            ct: ct);

        return new PaginatedDto<TenantDto>
        {
            Items = items.Adapt<IEnumerable<TenantDto>>(),
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.ValidatedPageSize)
        };
    }
}