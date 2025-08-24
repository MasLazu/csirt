using System;
using MediatR;
using MeUi.Application.Models.ThreatCompliance;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetCurrentRiskLevel;

public class GetCurrentRiskLevelQuery : IRequest<RiskLevelDto>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}
