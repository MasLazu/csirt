using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Queries.GetActions;

public class GetActionsQueryHandler : IRequestHandler<GetActionsQuery, IEnumerable<ActionDto>>
{
    private readonly IRepository<Domain.Entities.Action> _actionRepository;

    public GetActionsQueryHandler(IRepository<Domain.Entities.Action> actionRepository)
    {
        _actionRepository = actionRepository;
    }

    public async Task<IEnumerable<ActionDto>> Handle(GetActionsQuery request, CancellationToken ct)
    {
        var actions = await _actionRepository.GetAllAsync(ct);

        return actions.OrderBy(a => a.Name).Adapt<List<ActionDto>>();
    }
}