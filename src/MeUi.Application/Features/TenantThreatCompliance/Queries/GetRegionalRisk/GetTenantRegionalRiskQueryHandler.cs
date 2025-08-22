using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetRegionalRisk;

public class GetTenantRegionalRiskQueryHandler : IRequestHandler<GetTenantRegionalRiskQuery, List<RegionalRiskDto>>
{
    private readonly ITenantThreatComplianceRepository _repository;

    public GetTenantRegionalRiskQueryHandler(ITenantThreatComplianceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RegionalRiskDto>> Handle(GetTenantRegionalRiskQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetRegionalRiskAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}