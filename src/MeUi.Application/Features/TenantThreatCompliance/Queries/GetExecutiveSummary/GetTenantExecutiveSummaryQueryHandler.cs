using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetExecutiveSummary;

public class GetTenantExecutiveSummaryQueryHandler : IRequestHandler<GetTenantExecutiveSummaryQuery, List<ExecutiveSummaryDto>>
{
    private readonly ITenantThreatComplianceRepository _repository;

    public GetTenantExecutiveSummaryQueryHandler(ITenantThreatComplianceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ExecutiveSummaryDto>> Handle(GetTenantExecutiveSummaryQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetExecutiveSummaryAsync(request.TenantId, request.Start, request.End, cancellationToken);
    }
}