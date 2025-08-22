using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIncident;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIncident.Queries.GetIncidentSeverityDistribution;

public class GetTenantIncidentSeverityDistributionQueryHandler : IRequestHandler<GetTenantIncidentSeverityDistributionQuery, List<IncidentSeverityDistributionDto>>
{
    private readonly ITenantThreatIncidentRepository _repository;

    public GetTenantIncidentSeverityDistributionQueryHandler(ITenantThreatIncidentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<IncidentSeverityDistributionDto>> Handle(GetTenantIncidentSeverityDistributionQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetIncidentSeverityDistributionAsync(request.TenantId, request.Start, request.End, cancellationToken);
    }
}