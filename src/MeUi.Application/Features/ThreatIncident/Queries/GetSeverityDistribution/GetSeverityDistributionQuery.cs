using System;
using MediatR;
using MeUi.Application.Models.ThreatIncident;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIncident.Queries.GetSeverityDistribution;

public class GetSeverityDistributionQuery : IRequest<List<SeverityDistributionDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}
