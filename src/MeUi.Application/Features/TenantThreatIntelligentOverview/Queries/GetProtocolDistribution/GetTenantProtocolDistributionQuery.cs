using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetProtocolDistribution;

public class GetTenantProtocolDistributionQuery : IRequest<List<ProtocolDistributionDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public GetTenantProtocolDistributionQuery() { }

    public GetTenantProtocolDistributionQuery(Guid tenantId, DateTime startTime, DateTime endTime)
    {
        TenantId = tenantId;
        StartTime = startTime;
        EndTime = endTime;
    }
}