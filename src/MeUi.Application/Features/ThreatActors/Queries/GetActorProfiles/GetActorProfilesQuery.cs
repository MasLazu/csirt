using System;
using System.Collections.Generic;
using MediatR;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorProfiles;

public class GetActorProfilesQuery : IRequest<List<ActorProfileDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int Limit { get; init; } = 30;

    public GetActorProfilesQuery() { }
    public GetActorProfilesQuery(DateTime start, DateTime end, int limit = 30)
    {
        Start = start;
        End = end;
        Limit = limit;
    }
}
