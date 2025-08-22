using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatNetwork;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatNetwork.Queries.GetAsnNetworkAnalysis;

public class GetTenantAsnNetworkAnalysisQuery : IRequest<List<AsnNetworkDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 20;
}