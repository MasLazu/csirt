using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatCompliance;
using System;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetComplianceScore;

public class GetTenantComplianceScoreQuery : IRequest<ComplianceScoreDto>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public GetTenantComplianceScoreQuery() { }

    public GetTenantComplianceScoreQuery(Guid tenantId, DateTime start, DateTime end)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
    }
}