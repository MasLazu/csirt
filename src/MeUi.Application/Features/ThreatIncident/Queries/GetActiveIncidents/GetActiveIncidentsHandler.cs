using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIncident;

namespace MeUi.Application.Features.ThreatIncident.Queries.GetActiveIncidents;

public class GetActiveIncidentsHandler : IRequestHandler<GetActiveIncidentsQuery, List<IncidentSummaryDto>>
{
    private readonly IThreatIncidentRepository _repo;

    public GetActiveIncidentsHandler(IThreatIncidentRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<IncidentSummaryDto>> Handle(GetActiveIncidentsQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetActiveIncidentsAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
