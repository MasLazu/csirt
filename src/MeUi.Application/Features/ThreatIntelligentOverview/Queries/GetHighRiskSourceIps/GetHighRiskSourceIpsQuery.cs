using MediatR;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetHighRiskSourceIps;

public class GetHighRiskSourceIpsQuery : IRequest<List<HighRiskSourceIpDto>>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Limit { get; set; }

    public GetHighRiskSourceIpsQuery() { }

    public GetHighRiskSourceIpsQuery(DateTime startTime, DateTime endTime, int limit)
    {
        StartTime = startTime;
        EndTime = endTime;
        Limit = limit;
    }
}
