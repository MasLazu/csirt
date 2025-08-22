using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatNetwork.Queries.GetHighRiskIpReputation;

public class GetTenantHighRiskIpReputationQueryHandler : IRequestHandler<GetTenantHighRiskIpReputationQuery, List<HighRiskIpDto>>
{
    private readonly ITenantThreatNetworkRepository _repository;

    public GetTenantHighRiskIpReputationQueryHandler(ITenantThreatNetworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<HighRiskIpDto>> Handle(GetTenantHighRiskIpReputationQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetHighRiskIpReputationAsync(
            request.TenantId,
            request.Start,
            request.End,
            request.Limit,
            cancellationToken);
    }
}