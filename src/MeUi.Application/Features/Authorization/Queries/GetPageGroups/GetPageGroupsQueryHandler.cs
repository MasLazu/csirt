using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Domain.Entities;
using Mapster;

namespace MeUi.Application.Features.Authorization.Queries.GetPageGroups;

public class GetPageGroupsQueryHandler : IRequestHandler<GetPageGroupsQuery, IEnumerable<PageGroupDto>>
{
    private readonly IRepository<PageGroup> _pageGroupRepository;
    private readonly IRepository<Page> _pageRepository;

    public GetPageGroupsQueryHandler(
        IRepository<PageGroup> pageGroupRepository,
        IRepository<Page> pageRepository)
    {
        _pageGroupRepository = pageGroupRepository;
        _pageRepository = pageRepository;
    }

    public async Task<IEnumerable<PageGroupDto>> Handle(GetPageGroupsQuery request, CancellationToken ct)
    {
        IEnumerable<PageGroup> pageGroups = await _pageGroupRepository.GetAllAsync(ct);
        IEnumerable<Page> pages = await _pageRepository.GetAllAsync(ct);

        foreach (PageGroup pageGroup in pageGroups)
        {
            pageGroup.Pages = pages.Where(p => p.PageGroupId == pageGroup.Id).ToHashSet();
        }

        return pageGroups.Adapt<IEnumerable<PageGroupDto>>();
    }
}