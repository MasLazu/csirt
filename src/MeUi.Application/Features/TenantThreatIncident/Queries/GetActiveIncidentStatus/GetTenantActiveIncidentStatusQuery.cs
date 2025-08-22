using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatIncident;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatIncident.Queries.GetActiveIncidentStatus;

public class GetTenantActiveIncidentStatusQuery : IRequest<List<IncidentStatusDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 30;

    public GetTenantActiveIncidentStatusQuery() { }

    public GetTenantActiveIncidentStatusQuery(Guid tenantId, DateTime start, DateTime end, int limit = 30)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Limit = limit;
    }
}