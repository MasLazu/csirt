using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantRolesPaginated;

public class GetTenantRolesPaginatedQueryHandler : IRequestHandler<GetTenantRolesPaginatedQuery, PaginatedDto<TenantRoleDto>>
{
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantRolePermission> _tenantRolePermissionRepository;
    private readonly IRepository<TenantPermission> _tenantPermissionRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<MeUi.Domain.Entities.Action> _actionRepository;
    private readonly IRepository<PageTenantPermission> _pageTenantPermissionRepository;
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<PageGroup> _pageGroupRepository;

    public GetTenantRolesPaginatedQueryHandler(
        IRepository<TenantRole> tenantRoleRepository,
        IRepository<TenantRolePermission> tenantRolePermissionRepository,
        IRepository<TenantPermission> tenantPermissionRepository,
        IRepository<Resource> resourceRepository,
        IRepository<MeUi.Domain.Entities.Action> actionRepository,
        IRepository<PageTenantPermission> pageTenantPermissionRepository,
        IRepository<Page> pageRepository,
        IRepository<PageGroup> pageGroupRepository)
    {
        _tenantRoleRepository = tenantRoleRepository;
        _tenantRolePermissionRepository = tenantRolePermissionRepository;
        _tenantPermissionRepository = tenantPermissionRepository;
        _resourceRepository = resourceRepository;
        _actionRepository = actionRepository;
        _pageTenantPermissionRepository = pageTenantPermissionRepository;
        _pageRepository = pageRepository;
        _pageGroupRepository = pageGroupRepository;
    }

    public async Task<PaginatedDto<TenantRoleDto>> Handle(GetTenantRolesPaginatedQuery request, CancellationToken ct)
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
            "updatedat" => tr => (object)(tr.UpdatedAt == null ? DateTime.MinValue : tr.UpdatedAt),
            _ => tr => (object)(tr.Name ?? string.Empty) // Default sort by name
        };

        // Use efficient database-level pagination
        (IEnumerable<TenantRole> tenantRoles, int totalItems) = await _tenantRoleRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: request.IsDescending,
            skip: (request.ValidatedPage - 1) * request.ValidatedPageSize,
            take: request.ValidatedPageSize,
            ct: ct);

        var roleIds = tenantRoles.Select(r => r.Id).ToList();

        IEnumerable<TenantRolePermission> tenantRolePermissions = await _tenantRolePermissionRepository
            .FindAsync(trp => roleIds.Contains(trp.TenantRoleId), ct);

        IEnumerable<Guid> tenantPermissionIds = tenantRolePermissions.Select(trp => trp.TenantPermissionId).Distinct();
        IEnumerable<TenantPermission> tenantPermissions = await _tenantPermissionRepository
            .FindAsync(tp => tenantPermissionIds.Contains(tp.Id), ct);

        IEnumerable<string> resourceCodes = tenantPermissions.Select(p => p.ResourceCode).Distinct();
        IEnumerable<string> actionCodes = tenantPermissions.Select(p => p.ActionCode).Distinct();

        IEnumerable<Resource> resources = await _resourceRepository
            .FindAsync(r => resourceCodes.Contains(r.Code), ct);
        IEnumerable<Domain.Entities.Action> actions = await _actionRepository
            .FindAsync(a => actionCodes.Contains(a.Code), ct);

        var resourceLookup = resources.ToDictionary(r => r.Code);
        var actionLookup = actions.ToDictionary(a => a.Code);

        ILookup<Guid, Guid> rolePermissionLookup = tenantRolePermissions.ToLookup(trp => trp.TenantRoleId, trp => trp.TenantPermissionId);

        var permissionDto = tenantPermissions.Select(permission =>
        {
            PermissionDto dto = permission.Adapt<PermissionDto>();

            if (resourceLookup.TryGetValue(permission.ResourceCode, out Resource? resource))
            {
                dto.Resource = resource.Adapt<ResourceDto>();
            }

            if (actionLookup.TryGetValue(permission.ActionCode, out Domain.Entities.Action? action))
            {
                dto.Action = action.Adapt<ActionDto>();
            }

            return dto;
        }).ToList();

        var permissionLookup = permissionDto.ToDictionary(p => p.Id);

        IEnumerable<PageTenantPermission> pageTenantPermissions = await _pageTenantPermissionRepository
            .FindAsync(pp => tenantPermissionIds.Contains(pp.TenantPermissionId), ct);

        IEnumerable<Guid> pageIds = pageTenantPermissions.Select(pp => pp.PageId).Distinct();
        IEnumerable<Page> pages = await _pageRepository
            .FindAsync(p => pageIds.Contains(p.Id), ct);

        // Fetch page groups for the accessible pages
        IEnumerable<Guid> pageGroupIds = pages.Where(p => p.PageGroupId.HasValue).Select(p => p.PageGroupId!.Value).Distinct();
        IEnumerable<PageGroup> pageGroups = await _pageGroupRepository
            .FindAsync(pg => pageGroupIds.Contains(pg.Id), ct);

        // Create lookups for efficient data access
        ILookup<Guid, Guid> permissionPageLookup = pageTenantPermissions.ToLookup(pp => pp.TenantPermissionId, pp => pp.PageId);
        IEnumerable<PageDto> pageDto = pages.Adapt<IEnumerable<PageDto>>();
        var pageLookup = pageDto.ToDictionary(p => p.Id);
        ILookup<Guid, PageDto> pagesByGroupLookup = pageDto.Where(p => p.PageGroupId.HasValue).ToLookup(p => p.PageGroupId!.Value);

        IEnumerable<PageGroupDto> pageGroupDto = pageGroups.Adapt<IEnumerable<PageGroupDto>>();
        var pageGroupLookup = pageGroupDto.ToDictionary(pg => pg.Id);

        var tenantRoleDtoList = tenantRoles.Select(role =>
        {
            TenantRoleDto dto = role.Adapt<TenantRoleDto>();
            IEnumerable<Guid> rolePermissionIds = rolePermissionLookup[role.Id];

            dto.Permissions = rolePermissionIds.Where(permissionId => permissionLookup.ContainsKey(permissionId))
                                              .Select(permissionId => permissionLookup[permissionId])
                                              .ToList();

            // Set accessible page groups with their pages
            IEnumerable<Guid> accessiblePageIds = rolePermissionIds.SelectMany(permissionId => permissionPageLookup[permissionId]).Distinct();
            var accessiblePages = accessiblePageIds.Where(pageId => pageLookup.ContainsKey(pageId))
                                                   .Select(pageId => pageLookup[pageId])
                                                   .ToList();

            // Group pages by their page groups
            IEnumerable<Guid> accessiblePageGroupIds = accessiblePages.Where(p => p.PageGroupId.HasValue).Select(p => p.PageGroupId!.Value).Distinct();
            dto.AccessiblePageGroups = accessiblePageGroupIds.Where(pgId => pageGroupLookup.ContainsKey(pgId))
                                                            .Select(pgId =>
                                                            {
                                                                PageGroupDto pageGroup = pageGroupLookup[pgId];
                                                                // Create a new instance to avoid modifying the original
                                                                var pageGroupCopy = new PageGroupDto
                                                                {
                                                                    Id = pageGroup.Id,
                                                                    Code = pageGroup.Code,
                                                                    Name = pageGroup.Name,
                                                                    Icon = pageGroup.Icon,
                                                                    CreatedAt = pageGroup.CreatedAt,
                                                                    UpdatedAt = pageGroup.UpdatedAt,
                                                                    // Assign only accessible pages to this group
                                                                    Pages = pagesByGroupLookup[pgId]
                                                                        .Where(p => accessiblePageIds.Contains(p.Id))
                                                                        .ToList()
                                                                };
                                                                return pageGroupCopy;
                                                            })
                                                            .ToList();

            return dto;
        }).ToList();

        int totalPages = (int)Math.Ceiling((double)totalItems / request.ValidatedPageSize);

        return new PaginatedDto<TenantRoleDto>
        {
            Items = tenantRoleDtoList,
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }
}
