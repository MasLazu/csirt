using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetKpiTrend;

public class GetTenantKpiTrendQueryHandler : IRequestHandler<GetTenantKpiTrendQuery, List<KpiTrendPointDto>>
{
    private readonly ITenantThreatComplianceRepository _repository;

    public GetTenantKpiTrendQueryHandler(ITenantThreatComplianceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<KpiTrendPointDto>> Handle(GetTenantKpiTrendQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetKpiTrendAsync(request.TenantId, request.Start, request.End, cancellationToken);
    }
}