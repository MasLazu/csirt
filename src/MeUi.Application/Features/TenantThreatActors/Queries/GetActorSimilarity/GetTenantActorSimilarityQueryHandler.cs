using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorSimilarity;

public class GetTenantActorSimilarityQueryHandler : IRequestHandler<GetTenantActorSimilarityQuery, List<ActorSimilarityDto>>
{
    private readonly ITenantThreatActorsRepository _repository;

    public GetTenantActorSimilarityQueryHandler(ITenantThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorSimilarityDto>> Handle(GetTenantActorSimilarityQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetActorSimilarityAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}