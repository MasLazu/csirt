using System.Linq.Expressions;
using Mapster;
using MeUi.Application.Utilities;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUsersPaginated;

public class GetTenantUsersPaginatedQueryHandler : IRequestHandler<GetTenantUsersPaginatedQuery, PaginatedDto<TenantUserDto>>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;

    public GetTenantUsersPaginatedQueryHandler(
        IRepository<TenantUser> tenantUserRepository,
        IRepository<TenantRole> tenantRoleRepository,
        IRepository<TenantUserRole> tenantUserRoleRepository)
    {
        _tenantUserRepository = tenantUserRepository;
        _tenantRoleRepository = tenantRoleRepository;
        _tenantUserRoleRepository = tenantUserRoleRepository;
    }

    public async Task<PaginatedDto<TenantUserDto>> Handle(GetTenantUsersPaginatedQuery request, CancellationToken ct)
    {
        // Build the predicate for tenant filtering and optional filters
        Expression<Func<TenantUser, bool>> predicate = tu => tu.TenantId == request.TenantId;

        if (!string.IsNullOrEmpty(request.Search))
        {
            string search = request.Search;
            Expression<Func<TenantUser, bool>> searchPredicate = tu =>
                tu.TenantId == request.TenantId &&
                (tu.Name != null && tu.Name.Contains(search) ||
                 tu.Email != null && tu.Email.Contains(search) ||
                 tu.Username != null && tu.Username.Contains(search));
            predicate = predicate.And(searchPredicate);
        }

        if (request.IsSuspended.HasValue)
        {
            bool isSuspended = request.IsSuspended.Value;
            predicate = predicate.And(tu => tu.TenantId == request.TenantId && tu.IsSuspended == isSuspended);
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

        var tenantUserList = tenantUsers.ToList();
        var tenantUserIds = tenantUserList.Select(tu => tu.Id).ToList();

        IEnumerable<TenantUserRole> userRoles = await _tenantUserRoleRepository.FindAsync(tur => tenantUserIds.Contains(tur.TenantUserId), ct);
        IEnumerable<Guid> roleIds = userRoles.Select(ur => ur.TenantRoleId).Distinct();
        IEnumerable<TenantRole> roles = await _tenantRoleRepository.FindAsync(r => roleIds.Contains(r.Id) && r.TenantId == request.TenantId, ct);

        ILookup<Guid, Guid> userRoleLookup = userRoles.ToLookup(ur => ur.TenantUserId, ur => ur.TenantRoleId);

        IEnumerable<TenantRoleDto> roleDtos = roles.Adapt<IEnumerable<TenantRoleDto>>();
        var roleLookup = roleDtos.ToDictionary(r => r.Id);

        var items = tenantUserList.Select(tu =>
        {
            TenantUserDto dto = tu.Adapt<TenantUserDto>();
            IEnumerable<Guid> assignedRoleIds = userRoleLookup[tu.Id];
            dto.Roles = assignedRoleIds.Where(rid => roleLookup.ContainsKey(rid)).Select(rid => roleLookup[rid]).ToList();
            return dto;
        }).ToList();

        return new PaginatedDto<TenantUserDto>
        {
            Items = items,
            TotalItems = totalItems,
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / request.ValidatedPageSize)
        };
    }
}