using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetCurrentRiskLevel;

public class GetTenantCurrentRiskLevelQueryHandler : IRequestHandler<GetTenantCurrentRiskLevelQuery, RiskLevelDto>
{
    private readonly ITenantThreatComplianceRepository _repository;

    public GetTenantCurrentRiskLevelQueryHandler(ITenantThreatComplianceRepository repository)
    {
        _repository = repository;
    }

    public async Task<RiskLevelDto> Handle(GetTenantCurrentRiskLevelQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCurrentRiskLevelAsync(request.TenantId, request.Start, request.End, cancellationToken);
    }
}