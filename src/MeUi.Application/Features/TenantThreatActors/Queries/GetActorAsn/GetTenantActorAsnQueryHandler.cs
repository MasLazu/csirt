using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorAsn;

public class GetTenantActorAsnQueryHandler : IRequestHandler<GetTenantActorAsnQuery, List<ActorAsnDto>>
{
    private readonly ITenantThreatActorsRepository _repository;

    public GetTenantActorAsnQueryHandler(ITenantThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorAsnDto>> Handle(GetTenantActorAsnQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetActorAsnAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}