using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatNetwork;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatNetwork.Queries.GetProtocolTrends;

public class GetTenantProtocolTrendsQuery : IRequest<List<ProtocolTrendDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public TimeSpan Interval { get; set; }
}