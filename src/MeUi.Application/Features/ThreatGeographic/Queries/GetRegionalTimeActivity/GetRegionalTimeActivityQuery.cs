using System;
using MediatR;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetRegionalTimeActivity;

public class GetRegionalTimeActivityQuery : IRequest<List<RegionalTimeBucketDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int Limit { get; init; } = 40;
}
