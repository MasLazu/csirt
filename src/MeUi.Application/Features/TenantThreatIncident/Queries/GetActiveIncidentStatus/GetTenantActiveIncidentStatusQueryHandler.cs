using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIncident;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIncident.Queries.GetActiveIncidentStatus;

public class GetTenantActiveIncidentStatusQueryHandler : IRequestHandler<GetTenantActiveIncidentStatusQuery, List<IncidentStatusDto>>
{
    private readonly ITenantThreatIncidentRepository _repository;

    public GetTenantActiveIncidentStatusQueryHandler(ITenantThreatIncidentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<IncidentStatusDto>> Handle(GetTenantActiveIncidentStatusQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetActiveIncidentStatusAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}