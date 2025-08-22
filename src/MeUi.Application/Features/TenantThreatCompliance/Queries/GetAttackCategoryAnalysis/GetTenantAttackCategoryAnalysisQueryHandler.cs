using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetAttackCategoryAnalysis;

public class GetTenantAttackCategoryAnalysisQueryHandler : IRequestHandler<GetTenantAttackCategoryAnalysisQuery, List<AttackCategoryDto>>
{
    private readonly ITenantThreatComplianceRepository _repository;

    public GetTenantAttackCategoryAnalysisQueryHandler(ITenantThreatComplianceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AttackCategoryDto>> Handle(GetTenantAttackCategoryAnalysisQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAttackCategoryAnalysisAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}