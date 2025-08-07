using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Domain.Entities;
using Mapster;

namespace MeUi.Application.Features.Authorization.Queries.GetPages;

public class GetPagesQueryHandler : IRequestHandler<GetPagesQuery, IEnumerable<PageDto>>
{
    private readonly IRepository<Page> _pageRepository;

    public GetPagesQueryHandler(IRepository<Page> pageRepository)
    {
        _pageRepository = pageRepository;
    }

    public async Task<IEnumerable<PageDto>> Handle(GetPagesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Page> pages = await _pageRepository.GetAllAsync(cancellationToken);

        return pages.Adapt<IEnumerable<PageDto>>();
    }
}