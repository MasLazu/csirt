using MediatR;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetThreatCategoryAnalysis;

public class GetThreatCategoryAnalysisQuery : IRequest<List<ThreatCategoryAnalysisDto>>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public GetThreatCategoryAnalysisQuery() { }

    public GetThreatCategoryAnalysisQuery(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}
