using MediatR;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetTopTargetedPorts;

public class GetTopTargetedPortsQuery : IRequest<List<TargetedPortDto>>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Limit { get; set; }

    public GetTopTargetedPortsQuery() { }

    public GetTopTargetedPortsQuery(DateTime startTime, DateTime endTime, int limit)
    {
        StartTime = startTime;
        EndTime = endTime;
        Limit = limit;
    }
}
