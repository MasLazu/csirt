using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetHighRiskSourceIps;

public class GetTenantHighRiskSourceIpsQueryHandler : IRequestHandler<GetTenantHighRiskSourceIpsQuery, List<HighRiskSourceIpDto>>
{
    private readonly ITenantThreatIntelligentOverviewRepository _repository;

    public GetTenantHighRiskSourceIpsQueryHandler(ITenantThreatIntelligentOverviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<HighRiskSourceIpDto>> Handle(GetTenantHighRiskSourceIpsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetHighRiskSourceIpsAsync(request.TenantId, request.StartTime, request.EndTime, request.Limit, cancellationToken);
    }
}