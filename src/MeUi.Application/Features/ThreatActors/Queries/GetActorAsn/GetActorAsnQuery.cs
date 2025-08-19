using System;
using System.Collections.Generic;
using MediatR;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorAsn;

public class GetActorAsnQuery : IRequest<List<ActorAsnDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int Limit { get; init; } = 15;

    public GetActorAsnQuery() { }
    public GetActorAsnQuery(DateTime start, DateTime end, int limit = 15)
    {
        Start = start;
        End = end;
        Limit = limit;
    }
}
