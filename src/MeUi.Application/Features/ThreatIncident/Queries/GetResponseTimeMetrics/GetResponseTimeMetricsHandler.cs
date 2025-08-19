using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIncident;

namespace MeUi.Application.Features.ThreatIncident.Queries.GetResponseTimeMetrics;

public class GetResponseTimeMetricsHandler : IRequestHandler<GetResponseTimeMetricsQuery, List<ResponseTimeMetricDto>>
{
    private readonly IThreatIncidentRepository _repo;

    public GetResponseTimeMetricsHandler(IThreatIncidentRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ResponseTimeMetricDto>> Handle(GetResponseTimeMetricsQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetResponseTimeMetricsAsync(request.Start, request.End, cancellationToken);
    }
}
