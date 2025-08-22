using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetTimeOfDayPatterns;

public class GetTenantTimeOfDayPatternsQueryHandler : IRequestHandler<GetTenantTimeOfDayPatternsQuery, List<TimePeriodSeriesDto>>
{
    private readonly ITenantThreatTemporalRepository _repository;

    public GetTenantTimeOfDayPatternsQueryHandler(ITenantThreatTemporalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TimePeriodSeriesDto>> Handle(GetTenantTimeOfDayPatternsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTimeOfDayPatternsAsync(
            request.TenantId,
            request.Start,
            request.End,
            request.Interval,
            cancellationToken);
    }
}