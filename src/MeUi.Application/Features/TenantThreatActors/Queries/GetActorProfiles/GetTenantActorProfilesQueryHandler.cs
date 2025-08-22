using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorProfiles;

public class GetTenantActorProfilesQueryHandler : IRequestHandler<GetTenantActorProfilesQuery, List<ActorProfileDto>>
{
    private readonly ITenantThreatActorsRepository _repository;

    public GetTenantActorProfilesQueryHandler(ITenantThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorProfileDto>> Handle(GetTenantActorProfilesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetActorProfilesAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}