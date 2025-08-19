using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorPersistence;

public class GetActorPersistenceQueryHandler : IRequestHandler<GetActorPersistenceQuery, List<ActorPersistenceDto>>
{
    private readonly IThreatActorsRepository _repository;

    public GetActorPersistenceQueryHandler(IThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorPersistenceDto>> Handle(GetActorPersistenceQuery request, CancellationToken cancellationToken)
    {
        var res = await _repository.GetActorPersistenceAsync(request.Start, request.End, cancellationToken);
        return res;
    }
}
