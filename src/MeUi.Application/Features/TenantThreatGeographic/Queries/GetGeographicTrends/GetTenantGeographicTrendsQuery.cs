using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatGeographic;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetGeographicTrends;

public class GetTenantGeographicTrendsQuery : IRequest<List<GeographicTrendDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);

    public GetTenantGeographicTrendsQuery() { }

    public GetTenantGeographicTrendsQuery(Guid tenantId, DateTime start, DateTime end, TimeSpan interval)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Interval = interval;
    }
}