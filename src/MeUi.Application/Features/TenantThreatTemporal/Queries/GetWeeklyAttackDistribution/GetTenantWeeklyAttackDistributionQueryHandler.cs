using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetWeeklyAttackDistribution;

public class GetTenantWeeklyAttackDistributionQueryHandler : IRequestHandler<GetTenantWeeklyAttackDistributionQuery, List<DayOfWeekDto>>
{
    private readonly ITenantThreatTemporalRepository _repository;

    public GetTenantWeeklyAttackDistributionQueryHandler(ITenantThreatTemporalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DayOfWeekDto>> Handle(GetTenantWeeklyAttackDistributionQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetWeeklyAttackDistributionAsync(
            request.TenantId,
            request.Start,
            request.End,
            cancellationToken);
    }
}