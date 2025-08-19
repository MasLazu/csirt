using System;
using MediatR;
using MeUi.Application.Models.ThreatIncident;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIncident.Queries.GetResponseTimeMetrics;

public class GetResponseTimeMetricsQuery : IRequest<List<ResponseTimeMetricDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}
