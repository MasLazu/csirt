using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatGeographic;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetRegionalTimeZoneActivity;

public class GetTenantRegionalTimeZoneActivityQuery : IRequest<List<RegionalTimeZoneActivityDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 40;

    public GetTenantRegionalTimeZoneActivityQuery() { }

    public GetTenantRegionalTimeZoneActivityQuery(Guid tenantId, DateTime start, DateTime end, int limit = 40)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Limit = limit;
    }
}