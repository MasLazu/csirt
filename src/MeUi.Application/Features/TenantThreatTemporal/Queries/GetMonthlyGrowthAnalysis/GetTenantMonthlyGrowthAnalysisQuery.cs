using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatTemporal;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetMonthlyGrowthAnalysis;

public class GetTenantMonthlyGrowthAnalysisQuery : IRequest<List<MonthlyGrowthDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public int Limit { get; set; } = 40;
}