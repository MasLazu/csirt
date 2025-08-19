using System;
using MediatR;
using MeUi.Application.Models.ThreatIncident;
using System.Collections.Generic;

namespace MeUi.Application.Features.ThreatIncident.Queries.GetActiveIncidents;

public class GetActiveIncidentsQuery : IRequest<List<IncidentSummaryDto>>
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int Limit { get; init; } = 30;
}
