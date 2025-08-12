using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantPermissions;

public class GetTenantPermissionsQueryHandler : IRequestHandler<GetTenantPermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IRepository<TenantPermission> _tenantPermissionRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<Domain.Entities.Action> _actionRepository;

    public GetTenantPermissionsQueryHandler(
        IRepository<TenantPermission> tenantPermissionRepository,
        IRepository<Resource> resourceRepository,
        IRepository<Domain.Entities.Action> actionRepository)
    {
        _tenantPermissionRepository = tenantPermissionRepository;
        _resourceRepository = resourceRepository;
        _actionRepository = actionRepository;
    }

    public async Task<IEnumerable<PermissionDto>> Handle(GetTenantPermissionsQuery request, CancellationToken ct)
    {
        IEnumerable<TenantPermission> tenantPermissions = await _tenantPermissionRepository.GetAllAsync(ct);
        IEnumerable<Resource> resources = await _resourceRepository.GetAllAsync(ct);
        IEnumerable<Domain.Entities.Action> actions = await _actionRepository.GetAllAsync(ct);

        foreach (TenantPermission tenantPermission in tenantPermissions)
        {
            tenantPermission.Resource = resources.FirstOrDefault(r => r.Code == tenantPermission.ResourceCode);
            tenantPermission.Action = actions.FirstOrDefault(a => a.Code == tenantPermission.ActionCode);
        }

        return tenantPermissions.Adapt<IEnumerable<PermissionDto>>();
    }
}