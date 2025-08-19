using System;
using MediatR;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCountryAttackTrends;

public class GetCountryAttackTrendsQuery : IRequest<List<CountryAttackTrendPointDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}
