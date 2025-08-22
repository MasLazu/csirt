using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatNetwork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatNetwork.Queries.GetAsnNetworkAnalysis;

public class GetTenantAsnNetworkAnalysisQueryHandler : IRequestHandler<GetTenantAsnNetworkAnalysisQuery, List<AsnNetworkDto>>
{
    private readonly ITenantThreatNetworkRepository _repository;

    public GetTenantAsnNetworkAnalysisQueryHandler(ITenantThreatNetworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AsnNetworkDto>> Handle(GetTenantAsnNetworkAnalysisQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsnNetworkAnalysisAsync(
            request.TenantId,
            request.Start,
            request.End,
            request.Limit,
            cancellationToken);
    }
}