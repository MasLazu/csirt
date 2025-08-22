using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetThreatActivityTimeline;

public class GetTenantThreatActivityTimelineQuery : IRequest<List<TimelineDataPointDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Interval { get; set; }

    public GetTenantThreatActivityTimelineQuery() { }

    public GetTenantThreatActivityTimelineQuery(Guid tenantId, DateTime startTime, DateTime endTime, TimeSpan interval)
    {
        TenantId = tenantId;
        StartTime = startTime;
        EndTime = endTime;
        Interval = interval;
    }
}