using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorSimilarity;

public class GetTenantActorSimilarityQuery : IRequest<List<ActorSimilarityDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 25;

    public GetTenantActorSimilarityQuery() { }

    public GetTenantActorSimilarityQuery(Guid tenantId, DateTime start, DateTime end, int limit = 25)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Limit = limit;
    }
}