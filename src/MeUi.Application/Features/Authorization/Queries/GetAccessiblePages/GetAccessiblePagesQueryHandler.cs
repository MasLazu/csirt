using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Domain.Entities;
using Mapster;

namespace MeUi.Application.Features.Authorization.Queries.GetAccessiblePages;

public class GetAccessiblePagesQueryHandler : IRequestHandler<GetAccessiblePagesQuery, IEnumerable<PageGroupDto>>
{
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<PageGroup> _pageGroupRepository;
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<PagePermission> _pagePermissionRepository;

    public GetAccessiblePagesQueryHandler(
        IRepository<UserRole> userRoleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<Permission> permissionRepository,
        IRepository<PageGroup> pageGroupRepository,
        IRepository<Page> pageRepository,
        IRepository<PagePermission> pagePermissionRepository)
    {
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _pageGroupRepository = pageGroupRepository;
        _pageRepository = pageRepository;
        _pagePermissionRepository = pagePermissionRepository;
    }

    public async Task<IEnumerable<PageGroupDto>> Handle(GetAccessiblePagesQuery request, CancellationToken ct)
    {
        IEnumerable<UserRole> userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == request.UserId, ct);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToHashSet();

        IEnumerable<RolePermission> rolePermissions = await _rolePermissionRepository.FindAsync(rp => roleIds.Contains(rp.RoleId), ct);
        IEnumerable<Guid> permissionIds = rolePermissions.Select(rp => rp.PermissionId);

        IEnumerable<Permission> permissions = await _permissionRepository.FindAsync(p => permissionIds.Contains(p.Id), ct);

        IEnumerable<PagePermission> pagePermissions = (await _pagePermissionRepository
            .FindAsync(pra => permissionIds.Contains(pra.PermissionId), ct))
            .ToHashSet();

        var accessiblePageIds = pagePermissions.Select(pra => pra.PageId).ToHashSet();

        IEnumerable<Page> accessiblePages = await _pageRepository.FindAsync(p => accessiblePageIds.Contains(p.Id), ct);
        var pageGroupIds = accessiblePages.Select(p => p.PageGroupId!.Value).ToHashSet();

        IEnumerable<PageGroup> pageGroups = await _pageGroupRepository.FindAsync(pg => pageGroupIds.Contains(pg.Id), ct);

        foreach (PageGroup pageGroup in pageGroups)
        {
            pageGroup.Pages = accessiblePages.Where(p => p.PageGroupId == pageGroup.Id).ToHashSet();
        }

        return pageGroups.Adapt<IEnumerable<PageGroupDto>>();
    }
}