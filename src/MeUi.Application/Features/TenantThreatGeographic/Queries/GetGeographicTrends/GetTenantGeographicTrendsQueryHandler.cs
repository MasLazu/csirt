using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetGeographicTrends;

public class GetTenantGeographicTrendsQueryHandler : IRequestHandler<GetTenantGeographicTrendsQuery, List<GeographicTrendDto>>
{
    private readonly ITenantThreatGeographicRepository _repository;

    public GetTenantGeographicTrendsQueryHandler(ITenantThreatGeographicRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GeographicTrendDto>> Handle(GetTenantGeographicTrendsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetGeographicTrendsAsync(request.TenantId, request.Start, request.End, request.Interval, cancellationToken);
    }
}