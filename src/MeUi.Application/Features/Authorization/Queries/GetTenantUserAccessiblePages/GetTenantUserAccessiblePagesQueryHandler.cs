using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantUserAccessiblePages;

public class GetTenantUserAccessiblePagesQueryHandler : IRequestHandler<GetTenantUserAccessiblePagesQuery, IEnumerable<PageDto>>
{
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IRepository<TenantRolePermission> _tenantRolePermissionRepository;
    private readonly IRepository<PageTenantPermission> _pageTenantPermissionRepository;

    public GetTenantUserAccessiblePagesQueryHandler(
        IRepository<Page> pageRepository,
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IRepository<TenantRolePermission> rolePermissionRepository,
        IRepository<PageTenantPermission> pageTenantPermissionRepository)
    {
        _pageRepository = pageRepository;
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _tenantRolePermissionRepository = rolePermissionRepository;
        _pageTenantPermissionRepository = pageTenantPermissionRepository;
    }

    public async Task<IEnumerable<PageDto>> Handle(GetTenantUserAccessiblePagesQuery request, CancellationToken ct)
    {
        IEnumerable<Guid> tenantRole = await _tenantUserRoleRepository
            .FindAsync(tur => tur.TenantUserId == request.UserId, tr => tr.TenantRoleId, ct);

        IEnumerable<Guid> permissionIds = await _tenantRolePermissionRepository.FindAsync(
            rp => tenantRole.Contains(rp.TenantRoleId), trp => trp.TenantPermissionId, ct);

        IEnumerable<Guid> pageIds = await _pageTenantPermissionRepository
            .FindAsync(pp => permissionIds.Contains(pp.TenantPermissionId), pp => pp.PageId, ct);

        IEnumerable<Page> pages = await _pageRepository.FindAsync(
            p => pageIds.Contains(p.Id), ct: ct);

        return pages.Adapt<IEnumerable<PageDto>>();
    }
}