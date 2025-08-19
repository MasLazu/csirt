using MediatR;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetTopThreatCategories;

public class GetTopThreatCategoriesQuery : IRequest<List<TopCategoryDto>>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Limit { get; set; }

    public GetTopThreatCategoriesQuery() { }

    public GetTopThreatCategoriesQuery(DateTime startTime, DateTime endTime, int limit)
    {
        StartTime = startTime;
        EndTime = endTime;
        Limit = limit;
    }
}
