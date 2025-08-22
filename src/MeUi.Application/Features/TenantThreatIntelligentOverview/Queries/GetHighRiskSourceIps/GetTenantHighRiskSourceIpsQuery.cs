using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetHighRiskSourceIps;

public class GetTenantHighRiskSourceIpsQuery : IRequest<List<HighRiskSourceIpDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Limit { get; set; }

    public GetTenantHighRiskSourceIpsQuery() { }

    public GetTenantHighRiskSourceIpsQuery(Guid tenantId, DateTime startTime, DateTime endTime, int limit)
    {
        TenantId = tenantId;
        StartTime = startTime;
        EndTime = endTime;
        Limit = limit;
    }
}