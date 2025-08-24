using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorDistributionByCountry;

public class GetActorDistributionByCountryQueryHandler : IRequestHandler<GetActorDistributionByCountryQuery, List<ActorCountryDistributionDto>>
{
    private readonly IThreatActorsRepository _repository;

    public GetActorDistributionByCountryQueryHandler(IThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorCountryDistributionDto>> Handle(GetActorDistributionByCountryQuery request, CancellationToken cancellationToken)
    {
        List<ActorCountryDistributionDto> result = await _repository.GetActorDistributionByCountryAsync(request.Start, request.End, request.Limit, cancellationToken);
        return result;
    }
}
