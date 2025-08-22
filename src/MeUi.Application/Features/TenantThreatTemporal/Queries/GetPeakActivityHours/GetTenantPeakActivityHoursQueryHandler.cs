using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetPeakActivityHours;

public class GetTenantPeakActivityHoursQueryHandler : IRequestHandler<GetTenantPeakActivityHoursQuery, List<PeakActivityDto>>
{
    private readonly ITenantThreatTemporalRepository _repository;

    public GetTenantPeakActivityHoursQueryHandler(ITenantThreatTemporalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PeakActivityDto>> Handle(GetTenantPeakActivityHoursQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetPeakActivityHoursAsync(
            request.TenantId,
            request.Start,
            request.End,
            request.Limit,
            cancellationToken);
    }
}