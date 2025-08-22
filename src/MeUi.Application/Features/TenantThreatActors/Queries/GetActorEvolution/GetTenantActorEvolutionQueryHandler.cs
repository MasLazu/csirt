using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorEvolution;

public class GetTenantActorEvolutionQueryHandler : IRequestHandler<GetTenantActorEvolutionQuery, List<ActorEvolutionDto>>
{
    private readonly ITenantThreatActorsRepository _repository;

    public GetTenantActorEvolutionQueryHandler(ITenantThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorEvolutionDto>> Handle(GetTenantActorEvolutionQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetActorEvolutionAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}