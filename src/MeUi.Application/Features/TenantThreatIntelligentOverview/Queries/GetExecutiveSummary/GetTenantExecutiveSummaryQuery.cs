using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetExecutiveSummary;

public class GetTenantExecutiveSummaryQuery : IRequest<List<ExecutiveSummaryMetricDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public GetTenantExecutiveSummaryQuery() { }

    public GetTenantExecutiveSummaryQuery(Guid tenantId, DateTime startTime, DateTime endTime)
    {
        TenantId = tenantId;
        StartTime = startTime;
        EndTime = endTime;
    }
}