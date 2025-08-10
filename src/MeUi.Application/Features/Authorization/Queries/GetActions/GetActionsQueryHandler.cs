using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetActions;

public class GetActionsQueryHandler : IRequestHandler<GetActionsQuery, IEnumerable<ActionDto>>
{
    private readonly IRepository<Domain.Entities.Action> _actionRepository;
    private readonly IRepository<Permission> _permissionRepository;

    public GetActionsQueryHandler(
        IRepository<Domain.Entities.Action> actionRepository,
        IRepository<Permission> permissionRepository)
    {
        _actionRepository = actionRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<IEnumerable<ActionDto>> Handle(GetActionsQuery request, CancellationToken ct)
    {
        IEnumerable<string> actionCodes = await _permissionRepository.GetAllAsync(
            p => p.ActionCode, ct);

        IEnumerable<Domain.Entities.Action> actions = await _actionRepository.FindAsync(
            a => actionCodes.Contains(a.Code),
            ct: ct);

        return actions.OrderBy(a => a.Name).Adapt<List<ActionDto>>();
    }
}