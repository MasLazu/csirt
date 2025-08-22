using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.Get24HourAttackPattern;

public class GetTenant24HourAttackPatternQueryHandler : IRequestHandler<GetTenant24HourAttackPatternQuery, List<TimeSeriesPointDto>>
{
    private readonly ITenantThreatTemporalRepository _repository;

    public GetTenant24HourAttackPatternQueryHandler(ITenantThreatTemporalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TimeSeriesPointDto>> Handle(GetTenant24HourAttackPatternQuery request, CancellationToken cancellationToken)
    {
        return await _repository.Get24HourAttackPatternAsync(
            request.TenantId,
            request.Start,
            request.End,
            cancellationToken);
    }
}