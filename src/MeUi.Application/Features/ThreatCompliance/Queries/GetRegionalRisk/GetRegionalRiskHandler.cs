using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetRegionalRisk;

public class GetRegionalRiskHandler : IRequestHandler<GetRegionalRiskQuery, List<RegionalRiskDto>>
{
    private readonly IThreatComplianceRepository _repo;

    public GetRegionalRiskHandler(IThreatComplianceRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<RegionalRiskDto>> Handle(GetRegionalRiskQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetRegionalRiskAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
