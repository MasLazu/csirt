using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetResources;

public class GetResourcesQueryHandler : IRequestHandler<GetResourcesQuery, IEnumerable<ResourceDto>>
{
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<Permission> _permissionRepository;

    public GetResourcesQueryHandler(
        IRepository<Resource> resourceRepository,
        IRepository<Permission> permissionRepository)
    {
        _resourceRepository = resourceRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<IEnumerable<ResourceDto>> Handle(GetResourcesQuery request, CancellationToken ct)
    {
        IEnumerable<string> resourceCode = await _permissionRepository.GetAllAsync(
            p => p.ResourceCode, ct);

        IEnumerable<Resource> resources = await _resourceRepository.FindAsync(
            r => resourceCode.Contains(r.Code),
            ct: ct);

        return resources.OrderBy(r => r.Name).Adapt<List<ResourceDto>>();
    }
}