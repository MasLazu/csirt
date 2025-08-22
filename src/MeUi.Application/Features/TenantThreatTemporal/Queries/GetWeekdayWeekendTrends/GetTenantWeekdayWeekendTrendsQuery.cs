using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatTemporal;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetWeekdayWeekendTrends;

public class GetTenantWeekdayWeekendTrendsQuery : IRequest<List<TimeSeriesPointDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}