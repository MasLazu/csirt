using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatActors;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorDistributionByCountry;

public class GetTenantActorDistributionByCountryQuery : IRequest<List<ActorCountryDistributionDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 15;

    public GetTenantActorDistributionByCountryQuery() { }

    public GetTenantActorDistributionByCountryQuery(Guid tenantId, DateTime start, DateTime end, int limit = 15)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Limit = limit;
    }
}