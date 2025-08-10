using MediatR;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Features.Tenants.Models;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using Mapster;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantsPaginated;

public class GetTenantsPaginatedQueryHandler : IRequestHandler<GetTenantsPaginatedQuery, PaginatedResult<TenantDto>>
{
    private readonly IRepository<Tenant> _tenantRepository;

    public GetTenantsPaginatedQueryHandler(IRepository<Tenant> tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<PaginatedResult<TenantDto>> Handle(GetTenantsPaginatedQuery request, CancellationToken ct)
    {
        (IEnumerable<Tenant> items, int totalCount) = await _tenantRepository.GetPaginatedAsync(
            predicate: t => string.IsNullOrEmpty(request.SearchTerm) ||
                            t.Name.Contains(request.SearchTerm) ||
                            t.Description!.Contains(request.SearchTerm),
            orderBy: t => t.Name,
            orderByDescending: false,
            skip: request.Page * request.PageSize,
            take: request.PageSize,
            ct: ct);

        return new PaginatedResult<TenantDto>
        {
            Items = items.Adapt<IEnumerable<TenantDto>>(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
}