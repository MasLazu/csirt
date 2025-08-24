using MediatR;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetKpiTrend;

public class GetKpiTrendQuery : IRequest<List<KpiTrendPointDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
