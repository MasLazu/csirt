using MediatR;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetExecutiveSummary;

public class GetExecutiveSummaryQuery : IRequest<List<ExecutiveSummaryMetricDto>>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public GetExecutiveSummaryQuery() { }

    public GetExecutiveSummaryQuery(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}
