using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatIncident;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatIncident.Queries.GetResponseTimeMetrics;

public class GetTenantResponseTimeMetricsQuery : IRequest<List<ResponseTimeMetricDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public TimeSpan Interval { get; set; }
}