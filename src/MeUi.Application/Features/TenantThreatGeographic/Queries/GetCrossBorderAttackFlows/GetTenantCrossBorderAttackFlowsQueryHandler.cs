using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetCrossBorderAttackFlows;

public class GetTenantCrossBorderAttackFlowsQueryHandler : IRequestHandler<GetTenantCrossBorderAttackFlowsQuery, List<CrossBorderAttackFlowDto>>
{
    private readonly ITenantThreatGeographicRepository _repository;

    public GetTenantCrossBorderAttackFlowsQueryHandler(ITenantThreatGeographicRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CrossBorderAttackFlowDto>> Handle(GetTenantCrossBorderAttackFlowsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCrossBorderAttackFlowsAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}