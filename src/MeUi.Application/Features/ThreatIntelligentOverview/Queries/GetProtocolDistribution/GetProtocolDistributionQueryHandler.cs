using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetProtocolDistribution;

public class GetProtocolDistributionQueryHandler : IRequestHandler<GetProtocolDistributionQuery, List<ProtocolDistributionDto>>
{
    private readonly IThreatIntelligentOverviewRepository _repository;

    public GetProtocolDistributionQueryHandler(IThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProtocolDistributionDto>> Handle(GetProtocolDistributionQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetProtocolDistributionAsync(request.StartTime, request.EndTime, cancellationToken);
    }
}
