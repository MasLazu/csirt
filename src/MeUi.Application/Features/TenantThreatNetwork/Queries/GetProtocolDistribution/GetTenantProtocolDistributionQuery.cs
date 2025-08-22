using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatNetwork;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatNetwork.Queries.GetProtocolDistribution;

public class GetTenantProtocolDistributionQuery : IRequest<List<ProtocolDistributionDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}