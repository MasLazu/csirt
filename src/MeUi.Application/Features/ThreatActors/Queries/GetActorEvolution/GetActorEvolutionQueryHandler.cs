using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorEvolution;

public class GetActorEvolutionQueryHandler : IRequestHandler<GetActorEvolutionQuery, List<ActorEvolutionDto>>
{
    private readonly IThreatActorsRepository _repository;

    public GetActorEvolutionQueryHandler(IThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorEvolutionDto>> Handle(GetActorEvolutionQuery request, CancellationToken cancellationToken)
    {
        List<ActorEvolutionDto> res = await _repository.GetActorEvolutionAsync(request.Start, request.End, request.Limit, cancellationToken);
        return res;
    }
}
