using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatCompliance;
using System;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetCurrentRiskLevel;

public class GetTenantCurrentRiskLevelQuery : IRequest<RiskLevelDto>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public GetTenantCurrentRiskLevelQuery() { }

    public GetTenantCurrentRiskLevelQuery(Guid tenantId, DateTime start, DateTime end)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
    }
}