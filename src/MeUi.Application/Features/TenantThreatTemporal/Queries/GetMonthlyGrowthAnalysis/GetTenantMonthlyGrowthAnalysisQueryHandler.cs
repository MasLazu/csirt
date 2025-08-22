using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetMonthlyGrowthAnalysis;

public class GetTenantMonthlyGrowthAnalysisQueryHandler : IRequestHandler<GetTenantMonthlyGrowthAnalysisQuery, List<MonthlyGrowthDto>>
{
    private readonly ITenantThreatTemporalRepository _repository;

    public GetTenantMonthlyGrowthAnalysisQueryHandler(ITenantThreatTemporalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MonthlyGrowthDto>> Handle(GetTenantMonthlyGrowthAnalysisQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetMonthlyGrowthAnalysisAsync(
            request.TenantId,
            request.Limit,
            cancellationToken);
    }
}