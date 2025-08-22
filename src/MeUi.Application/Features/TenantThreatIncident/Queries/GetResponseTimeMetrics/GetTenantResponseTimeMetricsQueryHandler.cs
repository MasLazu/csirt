using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatIncident;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatIncident.Queries.GetResponseTimeMetrics;

public class GetTenantResponseTimeMetricsQueryHandler : IRequestHandler<GetTenantResponseTimeMetricsQuery, List<ResponseTimeMetricDto>>
{
    private readonly ITenantThreatIncidentRepository _repository;

    public GetTenantResponseTimeMetricsQueryHandler(ITenantThreatIncidentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ResponseTimeMetricDto>> Handle(GetTenantResponseTimeMetricsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetResponseTimeMetricsAsync(
            request.TenantId,
            request.Start,
            request.End,
            request.Interval,
            cancellationToken);
    }
}