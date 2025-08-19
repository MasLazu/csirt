using System;
using MediatR;
using MeUi.Application.Models.ThreatCompliance;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetRegionalRisk
{
    public class GetRegionalRiskQuery : IRequest<List<RegionalRiskDto>>
    {
        public DateTime Start { get; init; }
        public DateTime End { get; init; }
        public int Limit { get; init; } = 15;
    }
}
