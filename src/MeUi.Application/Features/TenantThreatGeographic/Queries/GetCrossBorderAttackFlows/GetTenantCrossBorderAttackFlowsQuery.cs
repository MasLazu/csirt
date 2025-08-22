using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatGeographic;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetCrossBorderAttackFlows;

public class GetTenantCrossBorderAttackFlowsQuery : IRequest<List<CrossBorderAttackFlowDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 30;

    public GetTenantCrossBorderAttackFlowsQuery() { }

    public GetTenantCrossBorderAttackFlowsQuery(Guid tenantId, DateTime start, DateTime end, int limit = 30)
    {
        TenantId = tenantId;
        Start = start;
        End = end;
        Limit = limit;
    }
}