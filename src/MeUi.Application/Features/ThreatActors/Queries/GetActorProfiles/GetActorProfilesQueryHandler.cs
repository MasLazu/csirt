using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorProfiles;

public class GetActorProfilesQueryHandler : IRequestHandler<GetActorProfilesQuery, List<ActorProfileDto>>
{
    private readonly IThreatActorsRepository _repository;

    public GetActorProfilesQueryHandler(IThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorProfileDto>> Handle(GetActorProfilesQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetActorProfilesAsync(request.Start, request.End, request.Limit, cancellationToken);
        return result;
    }
}
