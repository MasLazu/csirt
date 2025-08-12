using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantRolesPaginated;

public class GetTenantRolesPaginatedQueryHandler : IRequestHandler<GetTenantRolesPaginatedQuery, PaginatedDto<RoleDto>>
{
    private readonly IRepository<TenantRole> _tenantRoleRepository;

    public GetTenantRolesPaginatedQueryHandler(IRepository<TenantRole> tenantRoleRepository)
    {
        _tenantRoleRepository = tenantRoleRepository;
    }

    public async Task<PaginatedDto<RoleDto>> Handle(GetTenantRolesPaginatedQuery request, CancellationToken ct)
    {
        // Build the predicate for tenant filtering and optional search
        Expression<Func<TenantRole, bool>> predicate = tr => tr.TenantId == request.TenantId;

        if (!string.IsNullOrEmpty(request.Search))
        {
            predicate = tr => tr.TenantId == request.TenantId && 
                             (tr.Name.Contains(request.Search) || tr.Description.Contains(request.Search));
        }

        // Determine sort field and direction
        Expression<Func<TenantRole, object>> orderBy = request.SortBy?.ToLower() switch
        {
            "description" => tr => tr.Description ?? string.Empty,
            "createdat" => tr => tr.CreatedAt,
            "updatedat" => tr => tr.UpdatedAt,
            _ => tr => tr.Name // Default sort by name
        };

        // Use efficient database-level pagination
        (IEnumerable<TenantRole> tenantRoles, int totalItems) = await _tenantRoleRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: request.IsDescending,
            skip: (request.ValidatedPage - 1) * request.ValidatedPageSize,
            take: request.ValidatedPageSize,
            ct: ct);

        IEnumerable<RoleDto> rolesDtos = tenantRoles.Adapt<IEnumerable<RoleDto>>();

        return new PaginatedDto<RoleDto>
        {
            Items = rolesDtos,
            TotalItems = totalItems,
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / request.ValidatedPageSize)
        };
    }
}
