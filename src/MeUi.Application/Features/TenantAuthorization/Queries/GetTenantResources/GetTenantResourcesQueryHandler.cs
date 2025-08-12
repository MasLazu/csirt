using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantResources;

public class GetTenantResourcesQueryHandler : IRequestHandler<GetTenantResourcesQuery, IEnumerable<ResourceDto>>
{
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<TenantPermission> _tenantPermissionRepository;

    public GetTenantResourcesQueryHandler(
        IRepository<TenantPermission> tenantPermissionRepository,
        IRepository<Resource> resourceRepository)
    {
        _tenantPermissionRepository = tenantPermissionRepository;
        _resourceRepository = resourceRepository;
    }

    public async Task<IEnumerable<ResourceDto>> Handle(GetTenantResourcesQuery request, CancellationToken ct)
    {
        IEnumerable<string> resourceCodes = await _tenantPermissionRepository.GetAllAsync(
            tp => tp.ResourceCode, ct);

        IEnumerable<Resource> resources = await _resourceRepository.FindAsync(
            r => resourceCodes.Contains(r.Code),
            ct: ct);

        return resources.OrderBy(r => r.Name).Adapt<IEnumerable<ResourceDto>>();
    }
}