using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorSimilarity;

public class GetActorSimilarityQueryHandler : IRequestHandler<GetActorSimilarityQuery, List<ActorSimilarityDto>>
{
    private readonly IThreatActorsRepository _repository;

    public GetActorSimilarityQueryHandler(IThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorSimilarityDto>> Handle(GetActorSimilarityQuery request, CancellationToken cancellationToken)
    {
        List<ActorSimilarityDto> res = await _repository.GetActorSimilarityAsync(request.Start, request.End, request.Limit, cancellationToken);
        return res;
    }
}
