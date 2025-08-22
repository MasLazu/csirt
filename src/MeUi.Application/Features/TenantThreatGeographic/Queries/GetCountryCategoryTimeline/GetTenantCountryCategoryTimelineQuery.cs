using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatGeographic;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetCountryCategoryTimeline;

public class GetTenantCountryCategoryTimelineQuery : IRequest<List<CountryCategoryTimelineDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);
    public int TopCountries { get; set; } = 5;

    public GetTenantCountryCategoryTimelineQuery() { }

    public GetTenantCountryCategoryTimelineQuery(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, int topCountries = 5)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Interval = interval;
        TopCountries = topCountries;
    }
}