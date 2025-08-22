using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorProfiles;

public class GetTenantActorProfilesQuery : IRequest<List<ActorProfileDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 30;

    public GetTenantActorProfilesQuery() { }

    public GetTenantActorProfilesQuery(Guid tenantId, DateTime start, DateTime end, int limit = 30)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Limit = limit;
    }
}