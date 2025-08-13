using System.Linq.Expressions;
using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUsersPaginated;

public class GetTenantUsersPaginatedQueryHandler : IRequestHandler<GetTenantUsersPaginatedQuery, PaginatedDto<TenantUserDto>>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;

    public GetTenantUsersPaginatedQueryHandler(IRepository<TenantUser> tenantUserRepository)
    {
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<PaginatedDto<TenantUserDto>> Handle(GetTenantUsersPaginatedQuery request, CancellationToken ct)
    {
        // Build the predicate for tenant filtering and optional filters
        Expression<Func<TenantUser, bool>> predicate = tu => tu.TenantId == request.TenantId;

        if (!string.IsNullOrEmpty(request.Search))
        {
            predicate = tu => tu.TenantId == request.TenantId &&
                             (tu.Name != null && tu.Name.Contains(request.Search) ||
                              tu.Email != null && tu.Email.Contains(request.Search) ||
                              tu.Username != null && tu.Username.Contains(request.Search));
        }

        if (request.IsSuspended.HasValue)
        {
            bool isSuspended = request.IsSuspended.Value;
            predicate = tu => tu.TenantId == request.TenantId &&
                             tu.IsSuspended == isSuspended &&
                             (!string.IsNullOrEmpty(request.Search) ?
                                (tu.Name != null && tu.Name.Contains(request.Search) ||
                                 tu.Email != null && tu.Email.Contains(request.Search) ||
                                 tu.Username != null && tu.Username.Contains(request.Search)) : true);
        }

        // Determine sort field and direction
        Expression<Func<TenantUser, object>> orderBy = request.SortBy?.ToLower() switch
        {
            "email" => tu => tu.Email ?? string.Empty,
            "username" => tu => tu.Username ?? string.Empty,
            "issuspended" => tu => tu.IsSuspended,
            "createdat" => tu => tu.CreatedAt,
            "updatedat" => tu => (object)(tu.UpdatedAt == null ? DateTime.MinValue : tu.UpdatedAt),
            _ => tu => (object)(tu.Name ?? string.Empty) // Default sort by name
        };

        // Use efficient database-level pagination
        (IEnumerable<TenantUser> tenantUsers, int totalItems) = await _tenantUserRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: request.IsDescending,
            skip: (request.ValidatedPage - 1) * request.ValidatedPageSize,
            take: request.ValidatedPageSize,
            ct: ct);

        var tenantUserDtos = tenantUsers.Adapt<IEnumerable<TenantUserDto>>() ?? Enumerable.Empty<TenantUserDto>();

        return new PaginatedDto<TenantUserDto>
        {
            Items = tenantUserDtos.ToList(),
            TotalItems = totalItems,
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / request.ValidatedPageSize)
        };
    }
}