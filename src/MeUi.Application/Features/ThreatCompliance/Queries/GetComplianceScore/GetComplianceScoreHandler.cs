using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetComplianceScore
{
    public class GetComplianceScoreHandler : IRequestHandler<GetComplianceScoreQuery, ComplianceScoreDto>
    {
        private readonly IThreatComplianceRepository _repo;

        public GetComplianceScoreHandler(IThreatComplianceRepository repo)
        {
            _repo = repo;
        }

        public async Task<ComplianceScoreDto> Handle(GetComplianceScoreQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetComplianceScoreAsync(request.Start, request.End, cancellationToken);
        }
    }
}
