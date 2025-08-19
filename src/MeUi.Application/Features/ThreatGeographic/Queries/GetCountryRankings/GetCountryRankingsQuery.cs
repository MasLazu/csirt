using System;
using MediatR;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCountryRankings;

public class GetCountryRankingsQuery : IRequest<List<CountryAttackRankingDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int Limit { get; init; } = 20;
}
