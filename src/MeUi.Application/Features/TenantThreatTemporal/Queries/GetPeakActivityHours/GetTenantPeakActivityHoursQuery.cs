using MediatR;
using MeUi.Application.Models;
using MeUi.Application.Models.ThreatTemporal;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetPeakActivityHours;

public class GetTenantPeakActivityHoursQuery : IRequest<List<PeakActivityDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 50;
}