using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantActions;

public class GetTenantActionsQueryHandler : IRequestHandler<GetTenantActionsQuery, IEnumerable<ActionDto>>
{
    private readonly IRepository<Domain.Entities.Action> _actionRepository;
    private readonly IRepository<TenantPermission> _tenantPermissionRepository;

    public GetTenantActionsQueryHandler(
        IRepository<Domain.Entities.Action> actionRepository,
        IRepository<TenantPermission> tenantPermissionRepository)
    {
        _actionRepository = actionRepository;
        _tenantPermissionRepository = tenantPermissionRepository;
    }

    public async Task<IEnumerable<ActionDto>> Handle(GetTenantActionsQuery request, CancellationToken ct)
    {
        IEnumerable<string> actionCodes = await _tenantPermissionRepository.GetAllAsync(
            tp => tp.ActionCode, ct);

        IEnumerable<Domain.Entities.Action> actions = await _actionRepository.FindAsync(
            a => actionCodes.Contains(a.Code),
            ct: ct);

        return actions.Adapt<IEnumerable<ActionDto>>();
    }
}