using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetResources;

public class GetResourcesQueryHandler : IRequestHandler<GetResourcesQuery, IEnumerable<ResourceDto>>
{
    private readonly IRepository<Resource> _resourceRepository;

    public GetResourcesQueryHandler(IRepository<Resource> resourceRepository)
    {
        _resourceRepository = resourceRepository;
    }

    public async Task<IEnumerable<ResourceDto>> Handle(GetResourcesQuery request, CancellationToken ct)
    {
        IEnumerable<Resource> resources = await _resourceRepository.GetAllAsync(ct);

        return resources.OrderBy(r => r.Name).Adapt<List<ResourceDto>>();
    }
}