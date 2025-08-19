using System;
using System.Collections.Generic;
using MediatR;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Application.Features.ThreatActors.Queries.GetTopActorsActivityTimeline;

public class GetTopActorsActivityTimelineQuery : IRequest<List<ActorActivityTimelineDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public TimeSpan Interval { get; init; }
    public int Limit { get; init; } = 10;

    public GetTopActorsActivityTimelineQuery() { }
    public GetTopActorsActivityTimelineQuery(DateTime start, DateTime end, TimeSpan interval, int limit = 10)
    {
        Start = start;
        End = end;
        Interval = interval;
        Limit = limit;
    }
}
