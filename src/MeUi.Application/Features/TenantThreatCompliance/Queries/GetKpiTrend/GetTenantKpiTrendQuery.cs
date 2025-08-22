using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetKpiTrend;

public class GetTenantKpiTrendQuery : IRequest<List<KpiTrendPointDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public GetTenantKpiTrendQuery() { }

    public GetTenantKpiTrendQuery(Guid tenantId, DateTime start, DateTime end)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
    }
}