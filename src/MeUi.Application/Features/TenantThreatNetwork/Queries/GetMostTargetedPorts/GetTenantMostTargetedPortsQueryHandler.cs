using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatNetwork.Queries.GetMostTargetedPorts;

public class GetTenantMostTargetedPortsQueryHandler : IRequestHandler<GetTenantMostTargetedPortsQuery, List<TargetedPortDto>>
{
    private readonly ITenantThreatNetworkRepository _repository;

    public GetTenantMostTargetedPortsQueryHandler(ITenantThreatNetworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TargetedPortDto>> Handle(GetTenantMostTargetedPortsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetMostTargetedPortsAsync(
            request.TenantId,
            request.Start,
            request.End,
            request.Limit,
            cancellationToken);
    }
}