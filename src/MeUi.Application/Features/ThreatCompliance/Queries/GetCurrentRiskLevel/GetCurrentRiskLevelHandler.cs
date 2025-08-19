using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetCurrentRiskLevel
{
    public class GetCurrentRiskLevelHandler : IRequestHandler<GetCurrentRiskLevelQuery, RiskLevelDto>
    {
        private readonly IThreatComplianceRepository _repo;

        public GetCurrentRiskLevelHandler(IThreatComplianceRepository repo)
        {
            _repo = repo;
        }

        public async Task<RiskLevelDto> Handle(GetCurrentRiskLevelQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetCurrentRiskLevelAsync(request.Start, request.End, cancellationToken);
        }
    }
}
