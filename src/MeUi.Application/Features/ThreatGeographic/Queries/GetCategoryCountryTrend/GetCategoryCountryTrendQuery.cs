using System;
using MediatR;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCategoryCountryTrend;

public class GetCategoryCountryTrendQuery : IRequest<List<CategoryCountryTrendPointDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}
