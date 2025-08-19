using MediatR;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetTopSourceCountries;

public class GetTopSourceCountriesQuery : IRequest<List<TopCountryDto>>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Limit { get; set; }

    public GetTopSourceCountriesQuery() { }

    public GetTopSourceCountriesQuery(DateTime startTime, DateTime endTime, int limit)
    {
        StartTime = startTime;
        EndTime = endTime;
        Limit = limit;
    }
}
