using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetProtocolDistribution;

public class GetTenantProtocolDistributionQueryHandler : IRequestHandler<GetTenantProtocolDistributionQuery, List<ProtocolDistributionDto>>
{
    private readonly ITenantThreatIntelligentOverviewRepository _repository;

    public GetTenantProtocolDistributionQueryHandler(ITenantThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProtocolDistributionDto>> Handle(GetTenantProtocolDistributionQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetProtocolDistributionAsync(request.TenantId, request.StartTime, request.EndTime, cancellationToken);
    }
}