using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorAsn;

public class GetActorAsnQueryHandler : IRequestHandler<GetActorAsnQuery, List<ActorAsnDto>>
{
    private readonly IThreatActorsRepository _repository;

    public GetActorAsnQueryHandler(IThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorAsnDto>> Handle(GetActorAsnQuery request, CancellationToken cancellationToken)
    {
        List<ActorAsnDto> result = await _repository.GetActorAsnAsync(request.Start, request.End, request.Limit, cancellationToken);
        return result;
    }
}
