using MediatR;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetThreatActivityTimeline;

public class GetThreatActivityTimelineQuery : IRequest<List<TimelineDataPointDto>>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Interval { get; set; }

    public GetThreatActivityTimelineQuery() { }

    public GetThreatActivityTimelineQuery(DateTime startTime, DateTime endTime, TimeSpan interval)
    {
        StartTime = startTime;
        EndTime = endTime;
        Interval = interval;
    }
}
