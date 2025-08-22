using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatNetwork.Queries.GetProtocolDistribution;

public class GetTenantProtocolDistributionQueryHandler : IRequestHandler<GetTenantProtocolDistributionQuery, List<ProtocolDistributionDto>>
{
    private readonly ITenantThreatNetworkRepository _repository;

    public GetTenantProtocolDistributionQueryHandler(ITenantThreatNetworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProtocolDistributionDto>> Handle(GetTenantProtocolDistributionQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetProtocolDistributionAsync(
            request.TenantId,
            request.Start,
            request.End,
            cancellationToken);
    }
}