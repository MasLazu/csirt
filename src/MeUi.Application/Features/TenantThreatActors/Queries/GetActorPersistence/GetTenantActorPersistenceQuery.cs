using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorPersistence;

public class GetTenantActorPersistenceQuery : IRequest<List<ActorPersistenceDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public GetTenantActorPersistenceQuery() { }

    public GetTenantActorPersistenceQuery(Guid tenantId, DateTime start, DateTime end)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
    }
}