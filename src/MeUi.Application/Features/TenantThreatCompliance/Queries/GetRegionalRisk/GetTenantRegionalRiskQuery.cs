using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetRegionalRisk;

public class GetTenantRegionalRiskQuery : IRequest<List<RegionalRiskDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 15;

    public GetTenantRegionalRiskQuery() { }

    public GetTenantRegionalRiskQuery(Guid tenantId, DateTime start, DateTime end, int limit = 15)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Limit = limit;
    }
}