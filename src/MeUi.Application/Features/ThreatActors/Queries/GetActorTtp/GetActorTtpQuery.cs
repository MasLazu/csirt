using System;
using System.Collections.Generic;
using MediatR;
using MeUi.Application.Models.ThreatActors;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorTtp;

public class GetActorTtpQuery : IRequest<List<ActorTtpDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int Limit { get; init; } = 25;

    public GetActorTtpQuery() { }
    public GetActorTtpQuery(DateTime start, DateTime end, int limit = 25)
    {
        Start = start;
        End = end;
        Limit = limit;
    }
}
