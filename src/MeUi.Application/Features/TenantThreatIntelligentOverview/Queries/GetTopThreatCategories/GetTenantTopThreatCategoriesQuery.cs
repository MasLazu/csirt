using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetTopThreatCategories;

public class GetTenantTopThreatCategoriesQuery : IRequest<List<TopCategoryDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Limit { get; set; }

    public GetTenantTopThreatCategoriesQuery() { }

    public GetTenantTopThreatCategoriesQuery(Guid tenantId, DateTime startTime, DateTime endTime, int limit)
    {
        TenantId = tenantId;
        StartTime = startTime;
        EndTime = endTime;
        Limit = limit;
    }
}