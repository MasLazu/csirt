using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.Authorization.Queries.GetRolesPaginated;

public class GetRolesPaginatedQueryHandler : IRequestHandler<GetRolesPaginatedQuery, PaginatedDto<RoleDto>>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<MeUi.Domain.Entities.Action> _actionRepository;
    private readonly IRepository<PagePermission> _pagePermissionRepository;
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<PageGroup> _pageGroupRepository;

    public GetRolesPaginatedQueryHandler(
        IRepository<Role> roleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<Permission> permissionRepository,
        IRepository<Resource> resourceRepository,
        IRepository<MeUi.Domain.Entities.Action> actionRepository,
        IRepository<PagePermission> pagePermissionRepository,
        IRepository<Page> pageRepository,
        IRepository<PageGroup> pageGroupRepository)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _resourceRepository = resourceRepository;
        _actionRepository = actionRepository;
        _pagePermissionRepository = pagePermissionRepository;
        _pageRepository = pageRepository;
        _pageGroupRepository = pageGroupRepository;
    }

    public async Task<PaginatedDto<RoleDto>> Handle(GetRolesPaginatedQuery request, CancellationToken ct)
    {
        Expression<Func<Role, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            predicate = r => r.Name.Contains(request.Search) || r.Description.Contains(request.Search);
        }

        (IEnumerable<Role> roles, int totalItems) = await _roleRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: r => r.Name,
            skip: (request.Page - 1) * request.PageSize,
            take: request.PageSize,
            ct: ct);

        var roleIds = roles.Select(r => r.Id).ToList();

        IEnumerable<RolePermission> rolePermissions = await _rolePermissionRepository
            .FindAsync(rp => roleIds.Contains(rp.RoleId), ct);

        IEnumerable<Guid> permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct();
        IEnumerable<Permission> permissions = await _permissionRepository
            .FindAsync(p => permissionIds.Contains(p.Id), ct);

        IEnumerable<string> resourceCodes = permissions.Select(p => p.ResourceCode).Distinct();
        IEnumerable<string> actionCodes = permissions.Select(p => p.ActionCode).Distinct();

        IEnumerable<Resource> resources = await _resourceRepository
            .FindAsync(r => resourceCodes.Contains(r.Code), ct);
        IEnumerable<Domain.Entities.Action> actions = await _actionRepository
            .FindAsync(a => actionCodes.Contains(a.Code), ct);

        var resourceLookup = resources.ToDictionary(r => r.Code);
        var actionLookup = actions.ToDictionary(a => a.Code);

        ILookup<Guid, Guid> rolePermissionLookup = rolePermissions.ToLookup(rp => rp.RoleId, rp => rp.PermissionId);

        var permissionDto = permissions.Select(permission =>
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

        IEnumerable<PagePermission> pagePermissions = await _pagePermissionRepository
            .FindAsync(pp => permissionIds.Contains(pp.PermissionId), ct);

        IEnumerable<Guid> pageIds = pagePermissions.Select(pp => pp.PageId).Distinct();
        IEnumerable<Page> pages = await _pageRepository
            .FindAsync(p => pageIds.Contains(p.Id), ct);

        // Fetch page groups for the accessible pages
        IEnumerable<Guid> pageGroupIds = pages.Where(p => p.PageGroupId.HasValue).Select(p => p.PageGroupId!.Value).Distinct();
        IEnumerable<PageGroup> pageGroups = await _pageGroupRepository
            .FindAsync(pg => pageGroupIds.Contains(pg.Id), ct);

        // Create lookups for efficient data access
        ILookup<Guid, Guid> permissionPageLookup = pagePermissions.ToLookup(pp => pp.PermissionId, pp => pp.PageId);
        IEnumerable<PageDto> pageDto = pages.Adapt<IEnumerable<PageDto>>();
        var pageLookup = pageDto.ToDictionary(p => p.Id);
        ILookup<Guid, PageDto> pagesByGroupLookup = pageDto.Where(p => p.PageGroupId.HasValue).ToLookup(p => p.PageGroupId!.Value);

        IEnumerable<PageGroupDto> pageGroupDto = pageGroups.Adapt<IEnumerable<PageGroupDto>>();
        var pageGroupLookup = pageGroupDto.ToDictionary(pg => pg.Id);

        var roleDto = roles.Select(role =>
        {
            RoleDto dto = role.Adapt<RoleDto>();
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

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        return new PaginatedDto<RoleDto>
        {
            Items = roleDto,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }
}