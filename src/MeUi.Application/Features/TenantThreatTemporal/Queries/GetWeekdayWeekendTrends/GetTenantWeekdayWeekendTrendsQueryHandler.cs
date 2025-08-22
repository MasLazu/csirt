using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetWeekdayWeekendTrends;

public class GetTenantWeekdayWeekendTrendsQueryHandler : IRequestHandler<GetTenantWeekdayWeekendTrendsQuery, List<TimeSeriesPointDto>>
{
    private readonly ITenantThreatTemporalRepository _repository;

    public GetTenantWeekdayWeekendTrendsQueryHandler(ITenantThreatTemporalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TimeSeriesPointDto>> Handle(GetTenantWeekdayWeekendTrendsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetWeekdayWeekendTrendsAsync(
            request.TenantId,
            request.Start,
            request.End,
            cancellationToken);
    }
}