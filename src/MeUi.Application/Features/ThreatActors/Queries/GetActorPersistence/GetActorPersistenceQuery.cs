using System;
using System.Collections.Generic;
using MediatR;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorPersistence;

public class GetActorPersistenceQuery : IRequest<List<ActorPersistenceDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }

    public GetActorPersistenceQuery() { }
    public GetActorPersistenceQuery(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }
}
