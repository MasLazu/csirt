using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantPages;

public class GetTenantPagesQueryHandler : IRequestHandler<GetTenantPagesQuery, IEnumerable<PageDto>>
{
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<PageTenantPermission> _pageTenantPermissionRepository;

    public GetTenantPagesQueryHandler(
        IRepository<Page> pageRepository,
        IRepository<PageTenantPermission> pageTenantPermissionRepository)
    {
        _pageRepository = pageRepository;
        _pageTenantPermissionRepository = pageTenantPermissionRepository;
    }

    public async Task<IEnumerable<PageDto>> Handle(GetTenantPagesQuery request, CancellationToken ct)
    {
        IEnumerable<Guid> pageTenantPermissions = await _pageTenantPermissionRepository.GetAllAsync(pt => pt.PageId, ct);
        IEnumerable<Page> pages = await _pageRepository.FindAsync(p => pageTenantPermissions.Contains(p.Id), ct);

        return pages.Adapt<IEnumerable<PageDto>>();
    }
}