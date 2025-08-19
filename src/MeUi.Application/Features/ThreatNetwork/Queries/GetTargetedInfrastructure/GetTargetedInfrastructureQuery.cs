using MediatR;
using MeUi.Application.Models.ThreatNetwork;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetTargetedInfrastructure;

public class GetTargetedInfrastructureQuery : IRequest<List<TargetedInfrastructureDto>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Limit { get; set; } = 20;
}
