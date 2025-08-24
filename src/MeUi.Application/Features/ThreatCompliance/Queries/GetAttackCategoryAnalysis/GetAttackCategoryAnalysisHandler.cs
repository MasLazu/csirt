using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetAttackCategoryAnalysis;

public class GetAttackCategoryAnalysisHandler : IRequestHandler<GetAttackCategoryAnalysisQuery, List<AttackCategoryDto>>
{
    private readonly IThreatComplianceRepository _repo;

    public GetAttackCategoryAnalysisHandler(IThreatComplianceRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<AttackCategoryDto>> Handle(GetAttackCategoryAnalysisQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetAttackCategoryAnalysisAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
