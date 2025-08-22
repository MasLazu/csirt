using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetTopActorsActivityTimeline;

public class GetTenantTopActorsActivityTimelineQuery : IRequest<List<ActorActivityTimelineDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public TimeSpan Interval { get; set; }
    public int Limit { get; set; } = 10;

    public GetTenantTopActorsActivityTimelineQuery() { }

    public GetTenantTopActorsActivityTimelineQuery(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, int limit = 10)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Interval = interval;
        Limit = limit;
    }
}