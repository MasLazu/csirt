using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetHighRiskSourceIps;

public class GetHighRiskSourceIpsQueryHandler : IRequestHandler<GetHighRiskSourceIpsQuery, List<HighRiskSourceIpDto>>
{
    private readonly IThreatIntelligentOverviewRepository _repository;

    public GetHighRiskSourceIpsQueryHandler(IThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<HighRiskSourceIpDto>> Handle(GetHighRiskSourceIpsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetHighRiskSourceIpsAsync(request.StartTime, request.EndTime, request.Limit, cancellationToken);
    }
}
