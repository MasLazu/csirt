using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatIncident;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatIncident.Queries.GetIncidentSeverityDistribution;

public class GetTenantIncidentSeverityDistributionQuery : IRequest<List<IncidentSeverityDistributionDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public GetTenantIncidentSeverityDistributionQuery() { }

    public GetTenantIncidentSeverityDistributionQuery(Guid tenantId, DateTime start, DateTime end)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
    }
}