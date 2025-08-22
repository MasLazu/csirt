using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetExecutiveSummary;

public class GetTenantExecutiveSummaryQuery : IRequest<List<ExecutiveSummaryDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public GetTenantExecutiveSummaryQuery() { }

    public GetTenantExecutiveSummaryQuery(Guid tenantId, DateTime start, DateTime end)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
    }
}