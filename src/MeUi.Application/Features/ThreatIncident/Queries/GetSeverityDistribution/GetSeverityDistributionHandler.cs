using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIncident;

namespace MeUi.Application.Features.ThreatIncident.Queries.GetSeverityDistribution;

public class GetSeverityDistributionHandler : IRequestHandler<GetSeverityDistributionQuery, List<SeverityDistributionDto>>
{
    private readonly IThreatIncidentRepository _repo;

    public GetSeverityDistributionHandler(IThreatIncidentRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SeverityDistributionDto>> Handle(GetSeverityDistributionQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetSeverityDistributionAsync(request.Start, request.End, cancellationToken);
    }
}
