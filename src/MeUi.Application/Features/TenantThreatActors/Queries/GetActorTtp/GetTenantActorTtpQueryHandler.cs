using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorTtp;

public class GetTenantActorTtpQueryHandler : IRequestHandler<GetTenantActorTtpQuery, List<ActorTtpDto>>
{
    private readonly ITenantThreatActorsRepository _repository;

    public GetTenantActorTtpQueryHandler(ITenantThreatActorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ActorTtpDto>> Handle(GetTenantActorTtpQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetActorTtpAnalysisAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}