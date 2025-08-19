using MediatR;
using MeUi.Application.Models.ThreatIntelligentOverview;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetProtocolDistribution;

public class GetProtocolDistributionQuery : IRequest<List<ProtocolDistributionDto>>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public GetProtocolDistributionQuery() { }

    public GetProtocolDistributionQuery(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}
