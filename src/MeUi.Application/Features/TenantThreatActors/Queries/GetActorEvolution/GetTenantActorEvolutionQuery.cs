using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorEvolution;

public class GetTenantActorEvolutionQuery : IRequest<List<ActorEvolutionDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 25;

    public GetTenantActorEvolutionQuery() { }

    public GetTenantActorEvolutionQuery(Guid tenantId, DateTime start, DateTime end, int limit = 25)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Limit = limit;
    }
}