using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatTemporal;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetCampaignDurationAnalysis;

public class GetTenantCampaignDurationAnalysisQuery : IRequest<List<CampaignDurationDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 30;
}