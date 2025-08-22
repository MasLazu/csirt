using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatGeographic;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetCountryRankings;

public class GetTenantCountryRankingsQuery : IRequest<List<CountryRankingDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 20;

    public GetTenantCountryRankingsQuery() { }

    public GetTenantCountryRankingsQuery(Guid tenantId, DateTime start, DateTime end, int limit = 20)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Limit = limit;
    }
}