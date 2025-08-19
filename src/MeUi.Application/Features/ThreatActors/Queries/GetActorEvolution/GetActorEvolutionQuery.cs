using System;
using System.Collections.Generic;
using MediatR;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorEvolution;

public class GetActorEvolutionQuery : IRequest<List<ActorEvolutionDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int Limit { get; init; } = 25;

    public GetActorEvolutionQuery() { }
    public GetActorEvolutionQuery(DateTime start, DateTime end, int limit = 25)
    {
        Start = start;
        End = end;
        Limit = limit;
    }
}
