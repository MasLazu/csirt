using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatNetwork.Queries.GetMostTargetedInfrastructure;

public class GetTenantMostTargetedInfrastructureQueryHandler : IRequestHandler<GetTenantMostTargetedInfrastructureQuery, List<TargetedInfrastructureDto>>
{
    private readonly ITenantThreatNetworkRepository _repository;

    public GetTenantMostTargetedInfrastructureQueryHandler(ITenantThreatNetworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TargetedInfrastructureDto>> Handle(GetTenantMostTargetedInfrastructureQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetMostTargetedInfrastructureAsync(
            request.TenantId,
            request.Start,
            request.End,
            request.Limit,
            cancellationToken);
    }
}