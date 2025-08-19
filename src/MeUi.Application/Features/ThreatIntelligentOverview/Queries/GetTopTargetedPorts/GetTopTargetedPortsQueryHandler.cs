using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetTopTargetedPorts;

public class GetTopTargetedPortsQueryHandler : IRequestHandler<GetTopTargetedPortsQuery, List<TargetedPortDto>>
{
    private readonly IThreatIntelligentOverviewRepository _repository;

    public GetTopTargetedPortsQueryHandler(IThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TargetedPortDto>> Handle(GetTopTargetedPortsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTopTargetedPortsAsync(request.StartTime, request.EndTime, request.Limit, cancellationToken);
    }
}
