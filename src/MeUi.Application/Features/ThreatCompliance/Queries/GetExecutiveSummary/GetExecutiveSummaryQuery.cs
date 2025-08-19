using MediatR;
using MeUi.Application.Models.ThreatCompliance;
using System;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetExecutiveSummary
{
    public class GetExecutiveSummaryQuery : IRequest<List<ExecutiveSummaryDto>>
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
