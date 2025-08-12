using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetPermissions;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<Domain.Entities.Action> _actionRepository;

    public GetPermissionsQueryHandler(
        IRepository<Permission> permissionRepository,
        IRepository<Resource> resourceRepository,
        IRepository<Domain.Entities.Action> actionRepository)
    {
        _permissionRepository = permissionRepository;
        _resourceRepository = resourceRepository;
        _actionRepository = actionRepository;
    }

    public async Task<IEnumerable<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken ct)
    {
        IEnumerable<Permission> permissions = await _permissionRepository.GetAllAsync(ct);
        IEnumerable<Resource> resources = await _resourceRepository.GetAllAsync(ct);
        IEnumerable<Domain.Entities.Action> actions = await _actionRepository.GetAllAsync(ct);

        foreach (Permission permission in permissions)
        {
            permission.Resource = resources.FirstOrDefault(r => r.Code == permission.ResourceCode);
            permission.Action = actions.FirstOrDefault(a => a.Code == permission.ActionCode);
        }

        return permissions.Adapt<IEnumerable<PermissionDto>>();
    }
}