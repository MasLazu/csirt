using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Mapster;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetUserAccessiblePages;

public class GetUserAccessiblePagesQueryHandler : IRequestHandler<GetUserAccessiblePagesQuery, IEnumerable<PageGroupDto>>
{
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<PageGroup> _pageGroupRepository;
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<PagePermission> _pagePermissionRepository;

    public GetUserAccessiblePagesQueryHandler(
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

    public async Task<IEnumerable<PageGroupDto>> Handle(GetUserAccessiblePagesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<UserRole> userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == request.UserId, cancellationToken);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToHashSet();

        IEnumerable<RolePermission> rolePermissions = await _rolePermissionRepository.FindAsync(rp => roleIds.Contains(rp.RoleId), cancellationToken);
        IEnumerable<Guid> permissionIds = rolePermissions.Select(rp => rp.PermissionId);

        IEnumerable<Permission> permissions = await _permissionRepository.FindAsync(p => permissionIds.Contains(p.Id), cancellationToken);

        IEnumerable<PagePermission> pagePermissions = (await _pagePermissionRepository
            .FindAsync(pra => permissionIds.Contains(pra.PermissionId), cancellationToken))
            .ToHashSet();

        var accessiblePageIds = pagePermissions.Select(pra => pra.PageId).ToHashSet();

        IEnumerable<Page> accessiblePages = await _pageRepository.FindAsync(p => accessiblePageIds.Contains(p.Id), cancellationToken);
        var pageGroupIds = accessiblePages.Select(p => p.PageGroupId!.Value).ToHashSet();

        IEnumerable<PageGroup> pageGroups = await _pageGroupRepository.FindAsync(pg => pageGroupIds.Contains(pg.Id), cancellationToken);

        foreach (PageGroup pageGroup in pageGroups)
        {
            pageGroup.Pages = accessiblePages.Where(p => p.PageGroupId == pageGroup.Id).ToHashSet();
        }

        return pageGroups.Adapt<IEnumerable<PageGroupDto>>();
    }
}
