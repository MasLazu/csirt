using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorDistributionByCountry;

public class GetTenantActorDistributionByCountryQueryHandler : IRequestHandler<GetTenantActorDistributionByCountryQuery, List<ActorCountryDistributionDto>>
{
    private readonly ITenantThreatActorsRepository _repository;

    public GetTenantActorDistributionByCountryQueryHandler(ITenantThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorCountryDistributionDto>> Handle(GetTenantActorDistributionByCountryQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetActorDistributionByCountryAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}