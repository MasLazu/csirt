using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetTopTargetedPorts;

public class GetTenantTopTargetedPortsQueryHandler : IRequestHandler<GetTenantTopTargetedPortsQuery, List<TargetedPortDto>>
{
    private readonly ITenantThreatIntelligentOverviewRepository _repository;

    public GetTenantTopTargetedPortsQueryHandler(ITenantThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TargetedPortDto>> Handle(GetTenantTopTargetedPortsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTopTargetedPortsAsync(request.TenantId, request.StartTime, request.EndTime, request.Limit, cancellationToken);
    }
}