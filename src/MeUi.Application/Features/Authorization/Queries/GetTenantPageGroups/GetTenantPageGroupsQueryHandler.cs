using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantPageGroups;

public class GetTenantPageGroupsQueryHandler : IRequestHandler<GetTenantPageGroupsQuery, IEnumerable<PageGroupDto>>
{
    private readonly IRepository<PageGroup> _pageGroupRepository;
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<PageTenantPermission> _pageTenantPermissionRepository;

    public GetTenantPageGroupsQueryHandler(
        IRepository<PageGroup> pageGroupRepository,
        IRepository<Page> pageRepository)
    {
        _pageGroupRepository = pageGroupRepository;
        _pageRepository = pageRepository;
    }

    public async Task<IEnumerable<PageGroupDto>> Handle(GetTenantPageGroupsQuery request, CancellationToken ct)
    {
        IEnumerable<Guid> pageTenantPermissions = await _pageTenantPermissionRepository.GetAllAsync(pt => pt.PageId, ct);
        IEnumerable<Page> pages = await _pageRepository.FindAsync(p => pageTenantPermissions.Contains(p.Id), ct);
        IEnumerable<Guid> pageIds = pages.Select(p => p.Id);
        IEnumerable<PageGroup> pageGroups = await _pageGroupRepository.FindAsync(pg => pageIds.Contains(pg.Id), ct);

        pageGroups = pageGroups.Select(pg =>
        {
            pg.Pages = pg.Pages.Where(p => p.PageGroupId == pg.Id).ToHashSet();
            return pg;
        });

        return pageGroups.Adapt<IEnumerable<PageGroupDto>>();
    }
}