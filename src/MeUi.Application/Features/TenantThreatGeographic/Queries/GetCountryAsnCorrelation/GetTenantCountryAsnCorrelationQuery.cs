using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatGeographic;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetCountryAsnCorrelation;

public class GetTenantCountryAsnCorrelationQuery : IRequest<List<CountryAsnCorrelationDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 25;

    public GetTenantCountryAsnCorrelationQuery() { }

    public GetTenantCountryAsnCorrelationQuery(Guid tenantId, DateTime start, DateTime end, int limit = 25)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Limit = limit;
    }
}