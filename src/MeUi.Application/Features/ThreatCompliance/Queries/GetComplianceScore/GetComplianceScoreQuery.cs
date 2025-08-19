using MediatR;
using MeUi.Application.Models.ThreatCompliance;
using System;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetComplianceScore
{
    public class GetComplianceScoreQuery : IRequest<ComplianceScoreDto>
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
