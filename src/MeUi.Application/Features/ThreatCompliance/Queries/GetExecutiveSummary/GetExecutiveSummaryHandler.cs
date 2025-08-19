using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetExecutiveSummary
{
    public class GetExecutiveSummaryHandler : IRequestHandler<GetExecutiveSummaryQuery, List<ExecutiveSummaryDto>>
    {
        private readonly IThreatComplianceRepository _repo;

        public GetExecutiveSummaryHandler(IThreatComplianceRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ExecutiveSummaryDto>> Handle(GetExecutiveSummaryQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetExecutiveSummaryAsync(request.Start, request.End, cancellationToken);
        }
    }
}
