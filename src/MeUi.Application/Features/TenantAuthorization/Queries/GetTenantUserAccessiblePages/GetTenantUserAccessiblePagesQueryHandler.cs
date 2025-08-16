using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantUserAccessiblePages;

public class GetTenantUserAccessiblePagesQueryHandler : IRequestHandler<GetTenantUserAccessiblePagesQuery, IEnumerable<PageGroupDto>>
{
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IRepository<TenantRolePermission> _tenantRolePermissionRepository;
    private readonly IRepository<PageTenantPermission> _pageTenantPermissionRepository;
    private readonly IRepository<PageGroup> _pageGroupRepository;

    public GetTenantUserAccessiblePagesQueryHandler(
        IRepository<Page> pageRepository,
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IRepository<TenantRolePermission> rolePermissionRepository,
        IRepository<PageGroup> pageGroupRepository,
        IRepository<PageTenantPermission> pageTenantPermissionRepository)
    {
        _pageRepository = pageRepository;
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _tenantRolePermissionRepository = rolePermissionRepository;
        _pageTenantPermissionRepository = pageTenantPermissionRepository;
        _pageGroupRepository = pageGroupRepository;
    }

    public async Task<IEnumerable<PageGroupDto>> Handle(GetTenantUserAccessiblePagesQuery request, CancellationToken ct)
    {
        IEnumerable<Guid> tenantRole = await _tenantUserRoleRepository
            .FindAsync(tur => tur.TenantUserId == request.UserId, tr => tr.TenantRoleId, ct);

        IEnumerable<Guid> permissionIds = await _tenantRolePermissionRepository.FindAsync(
            rp => tenantRole.Contains(rp.TenantRoleId), trp => trp.TenantPermissionId, ct);

        IEnumerable<Guid> pageIds = await _pageTenantPermissionRepository
            .FindAsync(pp => permissionIds.Contains(pp.TenantPermissionId), pp => pp.PageId, ct);

        IEnumerable<Page> pages = await _pageRepository.FindAsync(
            p => pageIds.Contains(p.Id), ct: ct);

        IEnumerable<PageGroup> pageGroups = await _pageGroupRepository
            .FindAsync(pg => pages.Select(p => p.PageGroupId).Contains(pg.Id), ct);

        foreach (PageGroup pageGroup in pageGroups)
        {
            pageGroup.Pages = pages.Where(p => p.PageGroupId == pageGroup.Id)
                .OrderBy(p => p.Code)
                .ToList();
        }

        IEnumerable<PageGroup> orderedGroups = pageGroups.OrderBy(pg => pg.Code);
        return orderedGroups.Adapt<IEnumerable<PageGroupDto>>();
    }
}