using System;
using MediatR;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCrossBorderFlows;

public class GetCrossBorderFlowsQuery : IRequest<List<CrossBorderFlowDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int Limit { get; init; } = 30;
}
