using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorTtp;

public class GetActorTtpQueryHandler : IRequestHandler<GetActorTtpQuery, List<ActorTtpDto>>
{
    private readonly IThreatActorsRepository _repository;

    public GetActorTtpQueryHandler(IThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorTtpDto>> Handle(GetActorTtpQuery request, CancellationToken cancellationToken)
    {
        var res = await _repository.GetActorTtpAnalysisAsync(request.Start, request.End, request.Limit, cancellationToken);
        return res;
    }
}
