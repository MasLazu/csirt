using System;
using System.Collections.Generic;
using MediatR;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorDistributionByCountry;

public class GetActorDistributionByCountryQuery : IRequest<List<ActorCountryDistributionDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int Limit { get; init; } = 15;

    public GetActorDistributionByCountryQuery() { }
    public GetActorDistributionByCountryQuery(DateTime start, DateTime end, int limit = 15)
    {
        Start = start;
        End = end;
        Limit = limit;
    }
}
