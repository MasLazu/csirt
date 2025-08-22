using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorPersistence;

public class GetTenantActorPersistenceQueryHandler : IRequestHandler<GetTenantActorPersistenceQuery, List<ActorPersistenceDto>>
{
    private readonly ITenantThreatActorsRepository _repository;

    public GetTenantActorPersistenceQueryHandler(ITenantThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorPersistenceDto>> Handle(GetTenantActorPersistenceQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetActorPersistenceAsync(request.TenantId, request.Start, request.End, cancellationToken);
    }
}