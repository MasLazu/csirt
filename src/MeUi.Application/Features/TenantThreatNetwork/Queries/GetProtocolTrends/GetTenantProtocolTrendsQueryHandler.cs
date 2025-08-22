using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatNetwork.Queries.GetProtocolTrends;

public class GetTenantProtocolTrendsQueryHandler : IRequestHandler<GetTenantProtocolTrendsQuery, List<ProtocolTrendDto>>
{
    private readonly ITenantThreatNetworkRepository _repository;

    public GetTenantProtocolTrendsQueryHandler(ITenantThreatNetworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProtocolTrendDto>> Handle(GetTenantProtocolTrendsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetProtocolTrendsAsync(
            request.TenantId,
            request.Start,
            request.End,
            request.Interval,
            cancellationToken);
    }
}