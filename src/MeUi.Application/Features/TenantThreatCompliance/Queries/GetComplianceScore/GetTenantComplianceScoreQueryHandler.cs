using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetComplianceScore;

public class GetTenantComplianceScoreQueryHandler : IRequestHandler<GetTenantComplianceScoreQuery, ComplianceScoreDto>
{
    private readonly ITenantThreatComplianceRepository _repository;

    public GetTenantComplianceScoreQueryHandler(ITenantThreatComplianceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ComplianceScoreDto> Handle(GetTenantComplianceScoreQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetComplianceScoreAsync(request.TenantId, request.Start, request.End, cancellationToken);
    }
}