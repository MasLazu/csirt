using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetKpiTrend;

public class GetKpiTrendHandler : IRequestHandler<GetKpiTrendQuery, List<KpiTrendPointDto>>
{
    private readonly IThreatComplianceRepository _repo;

    public GetKpiTrendHandler(IThreatComplianceRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<KpiTrendPointDto>> Handle(GetKpiTrendQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetKpiTrendAsync(request.Start, request.End, cancellationToken);
    }
}
